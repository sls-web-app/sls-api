using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.UserDto;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;
using sls_borders.Models;
using sls_utils.EmailUtils;
using sls_borders.Enums;
using sls_utils.AuthUtils;

namespace sls_api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UserInviteController(IUserInviteRepo userInviteRepo, ITeamRepo teamRepo, IMapper mapper, IUserRepo userRepo, IConfiguration configuration) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<InviteUserDto>>> GetAllInvites()
    {
        var invites = await userInviteRepo.GetAllAsync();
        return Ok(invites);
    }

    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetUserDto>> GetInviteById(Guid id)
    {
        var invite = await userInviteRepo.GetByIdAsync(id);

        if (invite == null) return NotFound(new ErrorResponse { Message = $"Invite with ID {id} not found" });

        return Ok(invite);
    }

    [HttpPost("invite")]
    public async Task<ActionResult> InviteUser([FromBody] InviteUserDto inviteDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (await userRepo.EmailExistsAsync(inviteDto.Email))
            return Conflict(new ErrorResponse { Message = $"User with email {inviteDto.Email} already exists" });

        if (await teamRepo.GetByIdAsync(inviteDto.TeamId) == null)
            return NotFound(new ErrorResponse { Message = $"Team with ID {inviteDto.TeamId} not found" });

        try
        {
            var succeeded = Enum.TryParse(inviteDto.Role, out Role role);
            if (!succeeded) return BadRequest(new ErrorResponse { Message = $"Invalid role: {inviteDto.Role}" });

            var createdUser = await userRepo.CreateAsync(new CreateUserDto
            {
                Email = inviteDto.Email,
                Name = inviteDto.Name,
                Surname = inviteDto.Surname,
                Role = role,
                TeamId = inviteDto.TeamId,
                Password = string.Empty, // Password will be set up later
                ProfileImg = string.Empty
            });

            var userInvite = new UserInvite
            {
                Id = Guid.NewGuid(),
                UserId = createdUser.Id,
            };

            var createdInvite = await userInviteRepo.CreateAsync(userInvite);

            string DomainName = configuration.GetSection("WebsiteOptions")["DomainName"] ?? "localhost:3000";
            await EmailUtils.SendRegisterEmailAsync(
                emailConfig: configuration.GetSection("EmailSettings"),
                toEmail: createdUser.Email,
                userName: $"{createdUser.Name} {createdUser.Surname}",
                PasswordSetupUrl: $"https://{DomainName}/setup-password/{createdInvite.Id}"
            );

            return CreatedAtAction(nameof(GetInviteById), new { id = createdInvite.Id }, createdInvite);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
    }
    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> DeleteInvite(Guid id)
    {
        var invite = await userInviteRepo.GetByIdAsync(id);
        if (invite == null)
            return NotFound(new ErrorResponse { Message = $"Invite with ID {id} not found" });

        await userRepo.DeleteAsync(invite.UserId);

        var deleted = await userInviteRepo.DeleteAsync(id);
        if (!deleted)
            return NotFound(new ErrorResponse { Message = $"Invite with ID {id} not found" });
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("register/{inviteId:guid}")]
    public async Task<ActionResult> RegisterUser(Guid inviteId, [FromBody] RegisterUserDto registerDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var invite = await userInviteRepo.GetByIdAsync(inviteId);
        if (invite == null)
            return NotFound(new ErrorResponse { Message = $"Invite with ID {inviteId} not found" });

        var user = await userRepo.RegisterAsync(invite.UserId, registerDto.Password);
        var keyString = configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT key is not configured.");
        string token = JwtUtils.GenerateJwtToken(user.Id, user.Email, user.Role, keyString);

        await userInviteRepo.DeleteAsync(inviteId);
        return Ok(new LoginUserResponseDto { Token = token });
    }
}