using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using sls_api.Configuration;
using sls_api.Hubs;
using sls_borders.Data;
using sls_borders.Mappings;
using sls_borders.Repositories;
using sls_repos.Data;
using sls_utils.ImageUtils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(AdminProfile), typeof(TeamProfile), typeof(TournamentProfile), typeof(GameProfile), typeof(UserProfile), typeof(EditionProfile));

// Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//PostgresSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("sls-repos"))
);

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddRepositories();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT key is not configured."))),
            ValidateIssuer = false,
            ValidateAudience = false,
            
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Initialize the database with seed data
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.AddServer("http://localhost:8080")
               .WithTitle("SLS API Documentation")
               .WithTheme(ScalarTheme.BluePlanet)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// Enable CORS before any other middleware
app.UseCors("DevelopmentCorsPolicy");

// Configure static files serving for uploaded images
var uploadsPathConfig = builder.Configuration["ImageUpload:Path"] ?? "Uploads";
var uploadsPath = Path.IsPathRooted(uploadsPathConfig)
    ? uploadsPathConfig
    : Path.Combine(Directory.GetCurrentDirectory(), uploadsPathConfig);
Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});


// Remove HTTPS redirection in development when running in Docker
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<TournamentsHub>("/hubs/tournaments");

app.Run();