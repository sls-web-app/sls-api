using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sls_borders.Data;
using sls_borders.DTO.UserDto;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;

namespace sls_api.Controllers;

/// <summary>
/// API controller for user management.
/// Provides CRUD operations for users including creation, retrieval, updates, deletion, and email validation.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserRepo userRepo, ApplicationDbContext dbContext, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    /// <returns>A collection of all users.</returns>
    /// <response code="200">Returns the list of all users.</response>
    [HttpGet("get-all")]
    [ProducesResponseType<IEnumerable<GetUserDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetUserDto>>> GetAllUsers()
    {
        var users = await userRepo.GetAllAsync();
        return Ok(mapper.Map<List<GetUserDto>>(users));
    }

    /// <summary>
    /// Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user details if found, otherwise a 404 error.</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="404">Returns not found if the user does not exist.</response>
    [HttpGet("get-by-id/{id:guid}")]
    [ProducesResponseType<GetUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserDto>> GetUserById(Guid id)
    {
        var user = await userRepo.GetByIdAsync(id);

        if (user == null)
            return NotFound(new ErrorResponse { Message = $"User with ID {id} not found" });

        return Ok(mapper.Map<GetUserDto>(user));
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>The user details if found, otherwise a 404 error.</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="404">Returns not found if the user does not exist.</response>
    [HttpGet("get-by-email/{email}")]
    [ProducesResponseType<GetUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserDto>> GetUserByEmail(string email)
    {
        var user = await userRepo.GetByEmailAsync(email);

        if (user == null)
            return NotFound(new ErrorResponse { Message = $"User with email {email} not found" });

        return Ok(mapper.Map<GetUserDto>(user));
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="dto">The user data for creation.</param>
    /// <returns>The created user or validation/conflict/not found errors.</returns>
    /// <response code="201">Returns the newly created user.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="404">Returns not found if the referenced team does not exist.</response>
    /// <response code="409">Returns conflict if the email address is already in use.</response>
    [HttpPost("create")]
    [ProducesResponseType<GetUserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetUserDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (await userRepo.EmailExistsAsync(dto.Email))
            return Conflict(new ErrorResponse { Message = $"User with email {dto.Email} already exists" });

        if (await dbContext.Teams.FindAsync(dto.TeamId) == null)
            return NotFound(new ErrorResponse { Message = $"Team with ID {dto.TeamId} not found" });

        try
        {
            var createdUser = await userRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, mapper.Map<GetUserDto>(createdUser));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing user with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="dto">The updated user data.</param>
    /// <returns>The updated user or a 404 error if not found.</returns>
    /// <response code="200">Returns the updated user.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="404">Returns not found if the user does not exist.</response>
    [HttpPut("update/{id:guid}")]
    [ProducesResponseType<GetUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updatedUser = await userRepo.UpdateAsync(id, dto);

            if (updatedUser == null)
                return NotFound(new ErrorResponse { Message = $"User with ID {id} not found" });

            return Ok(mapper.Map<GetUserDto>(updatedUser));
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a user from the system. This operation requires Admin role authorization.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>No content if successful, or 404 if not found.</returns>
    /// <response code="204">Returns no content if the user was successfully deleted.</response>
    /// <response code="401">Returns unauthorized if not authenticated.</response>
    /// <response code="403">Returns forbidden if not authorized as Admin.</response>
    /// <response code="404">Returns not found if the user does not exist.</response>
    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var result = await userRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"User with ID {id} not found" });

        return NoContent();
    }

    /// <summary>
    /// Checks whether the specified email address is already in use by another user.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email exists in the system, otherwise false.</returns>
    /// <response code="200">Returns a boolean indicating whether the email exists.</response>
    [HttpGet("email-exists/{email}")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> CheckEmailExists(string email)
    {
        var exists = await userRepo.EmailExistsAsync(email);
        return Ok(exists);
    }
}

