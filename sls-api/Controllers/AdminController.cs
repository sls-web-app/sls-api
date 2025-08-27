using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.AdminDto;
using sls_borders.Repositories;
using sls_borders.DTO.ErrorDto;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using sls_borders.Models;

namespace sls_api.Controllers;

/// <summary>
/// API controller for administrator management.
/// Provides CRUD operations for administrators including creation, retrieval, updates, and deletion.
/// All operations require Admin role authorization.
/// </summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class AdminController(IAdminRepo adminRepo, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Retrieves all administrators from the system.
    /// </summary>
    /// <returns>A collection of administrator objects.</returns>
    /// <response code="200">Returns the list of all administrators.</response>
    /// <response code="401">Returns unauthorized if not authenticated.</response>
    /// <response code="403">Returns forbidden if not authorized as Admin.</response>
    [HttpGet("get-all")]
    [ProducesResponseType<IEnumerable<GetAdminDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<GetAdminDto>>> GetAllAdmins()
    {
        var admins = await adminRepo.GetAllAsync();
        var adminDtos = mapper.Map<List<GetAdminDto>>(admins);
        return Ok(adminDtos);
    }

    /// <summary>
    /// Retrieves a specific administrator by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the administrator.</param>
    /// <returns>The administrator details if found, otherwise a 404 error.</returns>
    /// <response code="200">Returns the administrator details.</response>
    /// <response code="401">Returns unauthorized if not authenticated.</response>
    /// <response code="403">Returns forbidden if not authorized as Admin.</response>
    /// <response code="404">Returns not found if the administrator does not exist.</response>
    [HttpGet("get-by-id/{id:guid}")]
    [ProducesResponseType<GetAdminDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetAdminDto>> GetAdminById(Guid id)
    {
        var admin = await adminRepo.GetByIdAsync(id);

        if (admin == null)
            return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

        var dto = mapper.Map<GetAdminDto>(admin);
        return Ok(dto);
    }

    /// <summary>
    /// Creates a new administrator in the system.
    /// </summary>
    /// <param name="createAdminDto">The administrator data for creation.</param>
    /// <returns>The created administrator or validation/conflict errors.</returns>
    /// <response code="201">Returns the newly created administrator.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="401">Returns unauthorized if not authenticated.</response>
    /// <response code="403">Returns forbidden if not authorized as Admin.</response>
    /// <response code="409">Returns conflict if the administrator already exists.</response>
    
    [HttpPost("create")]
    [ProducesResponseType<GetAdminDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetAdminDto>> CreateAdmin([FromBody] CreateAdminDto createAdminDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var admin = mapper.Map<Admin>(createAdminDto);
            var createdAdmin = await adminRepo.CreateAsync(admin, createAdminDto.Password);
            var responseDto = mapper.Map<GetAdminDto>(createdAdmin);
            return CreatedAtAction(nameof(GetAdminById), new { id = responseDto.Id }, responseDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing administrator with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the administrator to update.</param>
    /// <param name="updateAdminDto">The updated administrator data.</param>
    /// <returns>The updated administrator or error/404 responses.</returns>
    /// <response code="200">Returns the updated administrator.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="401">Returns unauthorized if not authenticated.</response>
    /// <response code="403">Returns forbidden if not authorized as Admin.</response>
    /// <response code="404">Returns not found if the administrator does not exist.</response>
    /// <response code="409">Returns conflict if there's a business logic violation.</response>
    [HttpPut("update/{id:guid}")]
    [ProducesResponseType<GetAdminDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetAdminDto>> UpdateAdmin(Guid id, [FromBody] UpdateAdminDto updateAdminDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updatedAdmin = await adminRepo.UpdateAsync(id, updateAdminDto);

            if (updatedAdmin == null)
                return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

            var responseDto = mapper.Map<GetAdminDto>(updatedAdmin);
            return Ok(responseDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes an administrator from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the administrator to delete.</param>
    /// <returns>No content if successful, or 404 if not found.</returns>
    /// <response code="204">Returns no content if the administrator was successfully deleted.</response>
    /// <response code="401">Returns unauthorized if not authenticated.</response>
    /// <response code="403">Returns forbidden if not authorized as Admin.</response>
    /// <response code="404">Returns not found if the administrator does not exist.</response>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAdmin(Guid id)
    {
        var result = await adminRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

        return NoContent();
    }
}

