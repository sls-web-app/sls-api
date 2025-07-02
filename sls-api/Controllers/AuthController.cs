using Microsoft.AspNetCore.Mvc;
using sls_borders.Repositories;
using sls_api.Utils;
using sls_borders.DTO.Admin;
using sls_borders.Enums;
using sls_borders.Models;


namespace sls_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAdminRepo adminRepo) : ControllerBase
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

        string token = JwtUtils.GenerateJwtToken(admin.Id, admin.Username, Role.Admin, "your-secret-key");
        return Ok(new LoginAdminResponseDto { Token = token });
    }
}
