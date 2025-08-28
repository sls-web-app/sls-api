using Microsoft.AspNetCore.Mvc;
using sls_borders.Repositories;
using sls_utils.AuthUtils;
using sls_borders.DTO.AdminDto;
using sls_borders.Enums;
using sls_borders.Models;
using sls_borders.DTO.ErrorDto;
using sls_borders.DTO.UserDto;

namespace sls_api.Controllers;

/// <summary>
/// API controller for authentication management.
/// Handles login operations for administrators.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Authentication")]
public class AuthController(IAdminRepo adminRepo, IUserRepo userRepo, IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Authenticates an administrator and returns a JWT token.
    /// </summary>
    /// <param name="loginAdminDto">The login credentials containing username and password.</param>
    /// <returns>A JWT token if authentication is successful, or an error response if credentials are invalid.</returns>
    /// <response code="200">Returns a JWT token for successful authentication.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="401">Returns unauthorized if credentials are invalid.</response>
    [HttpPost("admin/login")]
    [ProducesResponseType<LoginAdminResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromBody] LoginAdminDto loginAdminDto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        Admin? admin = await adminRepo.LoginAsync(loginAdminDto.Username, loginAdminDto.Password);
        if (admin == null) return Unauthorized(new ErrorResponse { Message = "Invalid username or password" });

        var keyString = configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT key is not configured.");
        string token = JwtUtils.GenerateJwtToken(admin.Id, admin.Username, Role.Admin, keyString);
        return Ok(new LoginAdminResponseDto { Token = token });
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        if(loginUserDto.Password == string.Empty)
            return BadRequest(new ErrorResponse { Message = "Password cannot be empty" });

        User? user = await userRepo.LoginAsync(loginUserDto.Email, loginUserDto.Password);
        if (user == null) return Unauthorized(new ErrorResponse { Message = "Invalid email or password" });

        var keyString = configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT key is not configured.");
        string token = JwtUtils.GenerateJwtToken(user.Id, user.Email, user.Role, keyString);
        return Ok(new LoginUserResponseDto { Token = token });
    }
}
