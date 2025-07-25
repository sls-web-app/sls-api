using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.Team;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;

namespace sls_api.Controllers;

/// <summary>
/// API controller for team management.
/// Provides CRUD operations for teams including creation, retrieval, updates, deletion, and tournament information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TeamController(ITeamRepo teamRepo, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Retrieves all teams from the system.
    /// </summary>
    /// <returns>A collection of all teams.</returns>
    /// <response code="200">Returns the list of all teams.</response>
    [HttpGet("get-all")]
    [ProducesResponseType<IEnumerable<GetTeamDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetTeamDto>>> GetAllTeams()
    {
        var teams = await teamRepo.GetAllAsync();
        return Ok(mapper.Map<List<GetTeamDto>>(teams));
    }

    /// <summary>
    /// Retrieves a specific team by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the team.</param>
    /// <returns>The team details if found, otherwise a 404 error.</returns>
    /// <response code="200">Returns the team details.</response>
    /// <response code="404">Returns not found if the team does not exist.</response>
    [HttpGet("get-by-id/{id:guid}")]
    [ProducesResponseType<GetTeamDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetTeamDto>> GetTeamById(Guid id)
    {
        var team = await teamRepo.GetByIdAsync(id);

        if (team == null)
            return NotFound(new ErrorResponse { Message = $"Team with ID {id} not found" });

        return Ok(mapper.Map<GetTeamDto>(team));
    }

    /// <summary>
    /// Retrieves information about tournaments in which the specified team has participated.
    /// </summary>
    /// <param name="id">The unique identifier of the team.</param>
    /// <returns>A list of tournaments or a 404 error if the team does not exist.</returns>
    /// <response code="200">Returns the list of tournaments for the team.</response>
    /// <response code="404">Returns not found if the team does not exist.</response>
    [HttpGet("get-tournaments/{id:guid}")]
    [ProducesResponseType<GetTeamTournamentsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetTeamTournamentsDto>> GetTeamTournaments(Guid id)
    {
        try
        {
            var team = await teamRepo.GetTeamTournamentsInfo(id);
            return Ok(mapper.Map<GetTeamTournamentsDto>(team));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new team in the system.
    /// </summary>
    /// <param name="dto">The team data for creation.</param>
    /// <returns>The created team or validation errors.</returns>
    /// <response code="201">Returns the newly created team.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    [HttpPost("create")]
    [ProducesResponseType<GetTeamDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var createdTeam = await teamRepo.CreateAsync(dto);
        return CreatedAtAction(nameof(GetTeamById), new { id = createdTeam.Id }, mapper.Map<GetTeamDto>(createdTeam));
    }

    /// <summary>
    /// Updates an existing team with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the team to update.</param>
    /// <param name="updateDto">The updated team data.</param>
    /// <returns>The updated team or a 404 error if not found.</returns>
    /// <response code="200">Returns the updated team.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="404">Returns not found if the team does not exist.</response>
    [HttpPut("update/{id:guid}")]
    [ProducesResponseType<GetTeamDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] UpdateTeamDto updateDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var updatedTeam = await teamRepo.UpdateAsync(id, updateDto);

        if (updatedTeam == null)
            return NotFound(new ErrorResponse { Message = $"Team with ID {id} not found." });

        return Ok(mapper.Map<GetTeamDto>(updatedTeam));
    }

    /// <summary>
    /// Deletes a team from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the team to delete.</param>
    /// <returns>No content if successful, or 404 if not found.</returns>
    /// <response code="204">Returns no content if the team was successfully deleted.</response>
    /// <response code="404">Returns not found if the team does not exist.</response>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var result = await teamRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Team with ID {id} not found." });

        return NoContent();
    }
}


