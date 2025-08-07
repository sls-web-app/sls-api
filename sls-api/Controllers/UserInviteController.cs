using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.UserDto;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;
using sls_borders.Models;
using sls_utils.EmailUtils;

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

        var userInvite = mapper.Map<UserInvite>(inviteDto);

        if (await userRepo.EmailExistsAsync(userInvite.Email))
            return Conflict(new ErrorResponse { Message = $"User with email {userInvite.Email} already exists" });

        if (await userInviteRepo.EmailExistsAsync(userInvite.Email))
            return Conflict(new ErrorResponse { Message = $"User invite with email {userInvite.Email} already exists" });

        if (await teamRepo.GetByIdAsync(userInvite.TeamId) == null)
            return NotFound(new ErrorResponse { Message = $"Team with ID {userInvite.TeamId} not found" });

        try
        {
            var createdInvite = await userInviteRepo.CreateAsync(userInvite);

            string DomainName = configuration.GetSection("WebsiteOptions")["DomainName"] ?? "localhost:3000";
            await EmailUtils.SendRegisterEmailAsync(
                emailConfig: configuration.GetSection("EmailSettings"),
                toEmail: userInvite.Email,
                userName: $"{userInvite.Name} {userInvite.Surname}",
                PasswordSetupUrl: $"https://${DomainName}/setup-password/{createdInvite.Id}"
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

        if (await userRepo.EmailExistsAsync(invite.Email))
            return Conflict(new ErrorResponse { Message = $"User with email {invite.Email} already exists" });

        var user = new CreateUserDto
        {
            Email = invite.Email,
            Name = invite.Name,
            Surname = invite.Surname,
            Role = invite.Role,
            TeamId = invite.TeamId,
            Password = registerDto.Password,
            ProfileImg = string.Empty
        };
        try
        {
            var createdUser = await userRepo.CreateAsync(user);
            await userInviteRepo.DeleteAsync(inviteId);
            return CreatedAtAction(
                "GetUserById", 
                "User",        
                new { id = createdUser.Id }, 
                mapper.Map<GetUserDto>(createdUser)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
    }
}