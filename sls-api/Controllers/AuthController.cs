using Microsoft.AspNetCore.Mvc;
using sls_borders.Repositories;
using sls_utils.AuthUtils;
using sls_borders.DTO.Admin;
using sls_borders.Enums;
using sls_borders.Models;
using sls_borders.DTO.ErrorDto;


namespace sls_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAdminRepo adminRepo, IConfiguration configuration) : ControllerBase
{
    // summary: Logowanie administratora.
    // param: dto – Dane logowania administratora.
    // returns: Token JWT lub kod błędu walidacji/nieautoryzowany.
    [HttpPost("admin/login")]
    public async Task<ActionResult> Login([FromBody] LoginAdminDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        Admin? admin = await adminRepo.LoginAsync(dto.Username, dto.Password);
        if (admin == null) return Unauthorized(new ErrorResponse { Message = "Invalid username or password" });

        var keyString = configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT key is not configured.");
        string token = JwtUtils.GenerateJwtToken(admin.Id, admin.Username, Role.Admin, keyString);
        return Ok(new LoginAdminResponseDto { Token = token });
    }
}
