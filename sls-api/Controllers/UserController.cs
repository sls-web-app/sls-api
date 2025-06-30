using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sls_borders.Data;
using sls_borders.DTO.UserDto;
using sls_borders.Repositories;

namespace sls_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserRepo userRepo, ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetUserDto>>> GetAllUsers()
    {
        var users = await userRepo.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetUserDto>> GetUserById(Guid id)
    {
        var user = await userRepo.GetByIdAsync(id);
        
        if (user == null)
            return NotFound($"User with ID {id} not found");

        return Ok(user);
    }

    [HttpGet("get-by-email/{email}")]
    public async Task<ActionResult<GetUserDto>> GetUserByEmail(string email)
    {
        var user = await userRepo.GetByEmailAsync(email);
        
        if (user == null)
            return NotFound($"User with email {email} not found");

        return Ok(user);
    }

    [HttpPost("create")]
    public async Task<ActionResult<GetUserDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (await userRepo.EmailExistsAsync(dto.Email))
            return Conflict($"User with email {dto.Email} already exists");
        if(await dbContext.Teams.FindAsync(dto.TeamId) == null)
            return BadRequest($"Team with ID {dto.TeamId} not found");

        try
        {
            var createdUser = await userRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<GetUserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedUser = await userRepo.UpdateAsync(id, dto);

            if (updatedUser == null)
                return NotFound($"User with ID {id} not found");

            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var result = await userRepo.DeleteAsync(id);
        
        if (!result)
            return NotFound($"User with ID {id} not found");

        return NoContent();
    }

    [HttpGet("email-exists/{email}")]
    public async Task<ActionResult<bool>> CheckEmailExists(string email)
    {
        var exists = await userRepo.EmailExistsAsync(email);
        return Ok(exists);
    }
}