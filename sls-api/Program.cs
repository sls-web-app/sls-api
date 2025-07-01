using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_api.Configuration;
using Scalar.AspNetCore;
using sls_borders.Mappings; 

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(AdminProfile), typeof(TeamProfile), typeof(TournamentProfile), typeof(GameProfile), typeof(UserProfile));

//PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("sls-repos"))
);

builder.Services.AddRepositories();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();