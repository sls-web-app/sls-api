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
        var admin = new Admin { Username = "admin1", PasswordHash = "", PasswordSalt = "" };
        var createdAdmin = await _adminRepo.CreateAsync(admin, "pass123");

        Assert.NotNull(createdAdmin);
        Assert.Equal("admin1", createdAdmin.Username);
        Assert.False(string.IsNullOrEmpty(createdAdmin.PasswordHash));
        Assert.False(string.IsNullOrEmpty(createdAdmin.PasswordSalt));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowIfUsernameTaken()
    {
        var admin = new Admin { Username = "admin2", PasswordHash = "", PasswordSalt = "" };
        await _adminRepo.CreateAsync(admin, "pass123");

        var duplicateAdmin = new Admin { Username = "admin2", PasswordHash = "", PasswordSalt = "" };
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _adminRepo.CreateAsync(duplicateAdmin, "otherpass"));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAdmins()
    {
        await _adminRepo.CreateAsync(new Admin { Username = "admin3", PasswordHash = "", PasswordSalt = "" }, "pass");
        await _adminRepo.CreateAsync(new Admin { Username = "admin4", PasswordHash = "", PasswordSalt = "" }, "pass");

        var admins = await _adminRepo.GetAllAsync();
        Assert.True(admins.Count >= 2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectAdmin()
    {
        var created = await _adminRepo.CreateAsync(new Admin { Username = "admin5", PasswordHash = "", PasswordSalt = "" }, "pass");
        var found = await _adminRepo.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal("admin5", found.Username);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAdminWithCorrectCredentials()
    {
        var created = await _adminRepo.CreateAsync(new Admin { Username = "admin6", PasswordHash = "", PasswordSalt = "" }, "secret");

        var admin = await _adminRepo.LoginAsync("admin6", "secret");
        Assert.NotNull(admin);
        Assert.Equal(created.Id, admin.Id);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNullWithWrongPassword()
    {
        await _adminRepo.CreateAsync(new Admin { Username = "admin7", PasswordHash = "", PasswordSalt = "" }, "rightpass");

        var admin = await _adminRepo.LoginAsync("admin7", "wrongpass");
        Assert.Null(admin);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUsernameAndPassword()
    {
        var created = await _adminRepo.CreateAsync(new Admin { Username = "admin8", PasswordHash = "", PasswordSalt = "" }, "oldpass");
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
        var created = await _adminRepo.CreateAsync(new Admin { Username = "admin9", PasswordHash = "", PasswordSalt = "" }, "pass");
        var result = await _adminRepo.DeleteAsync(created.Id);

        Assert.True(result);
        var found = await _adminRepo.GetByIdAsync(created.Id);
        Assert.Null(found);
    }
}