using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.Models;
using sls_repos.Repositories;
using AutoMapper;
using sls_borders.DTO.AdminDto;

namespace Tests;

public class AdminRepoTest
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly AdminRepo _adminRepo;

    public AdminRepoTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<sls_borders.Mappings.AdminProfile>(); 
        });
        _mapper = mapperConfig.CreateMapper();

        _adminRepo = new AdminRepo(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddAdmin()
    {
        var dto = new CreateAdminDto { Username = "admin1", Password = "pass123" };
        var admin = await _adminRepo.CreateAsync(dto);

        Assert.NotNull(admin);
        Assert.Equal("admin1", admin.Username);
        Assert.False(string.IsNullOrEmpty(admin.PasswordHash));
        Assert.False(string.IsNullOrEmpty(admin.PasswordSalt));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowIfUsernameTaken()
    {
        var dto = new CreateAdminDto { Username = "admin2", Password = "pass123" };
        await _adminRepo.CreateAsync(dto);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _adminRepo.CreateAsync(new CreateAdminDto { Username = "admin2", Password = "otherpass" }));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAdmins()
    {
        await _adminRepo.CreateAsync(new CreateAdminDto { Username = "admin3", Password = "pass" });
        await _adminRepo.CreateAsync(new CreateAdminDto { Username = "admin4", Password = "pass" });

        var admins = await _adminRepo.GetAllAsync();
        Assert.True(admins.Count >= 2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectAdmin()
    {
        var created = await _adminRepo.CreateAsync(new CreateAdminDto { Username = "admin5", Password = "pass" });
        var found = await _adminRepo.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal("admin5", found.Username);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAdminWithCorrectCredentials()
    {
        var dto = new CreateAdminDto { Username = "admin6", Password = "secret" };
        var created = await _adminRepo.CreateAsync(dto);

        var admin = await _adminRepo.LoginAsync("admin6", "secret");
        Assert.NotNull(admin);
        Assert.Equal(created.Id, admin.Id);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNullWithWrongPassword()
    {
        await _adminRepo.CreateAsync(new CreateAdminDto { Username = "admin7", Password = "rightpass" });

        var admin = await _adminRepo.LoginAsync("admin7", "wrongpass");
        Assert.Null(admin);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUsernameAndPassword()
    {
        var created = await _adminRepo.CreateAsync(new CreateAdminDto { Username = "admin8", Password = "oldpass" });
        var oldHash = created.PasswordHash; // Store the original hash

        var dto = new UpdateAdminDto { Username = "admin8new", Password = "newpass" };
        await _adminRepo.UpdateAsync(created.Id, dto);

        var updated = await _adminRepo.GetByIdAsync(created.Id);

        Assert.NotNull(updated);
        Assert.Equal("admin8new", updated.Username);
        Assert.NotEqual(oldHash, updated.PasswordHash); 
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldRemoveAdmin()
    {
        var created = await _adminRepo.CreateAsync(new CreateAdminDto { Username = "admin9", Password = "pass" });
        var result = await _adminRepo.DeleteAsync(created.Id);

        Assert.True(result);
        var found = await _adminRepo.GetByIdAsync(created.Id);
        Assert.Null(found);
    }
}