using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.TournamentDto;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;

namespace sls_api.Controllers;

/// <summary>
/// API controller for tournament management.
/// Provides CRUD operations for tournaments including creation, retrieval, updates, and deletion.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TournamentController(ITournamentRepo tournamentRepo, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Retrieves all tournaments from the system.
    /// </summary>
    /// <returns>A collection of tournament objects.</returns>
    /// <response code="200">Returns the list of all tournaments.</response>
    [HttpGet("get-all")]
    [ProducesResponseType<IEnumerable<GetTournamentDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetTournamentDto>>> GetAllTournaments()
    {
        var tournaments = await tournamentRepo.GetAllAsync();
        return Ok(mapper.Map<List<GetTournamentDto>>(tournaments));
    }

    /// <summary>
    /// Retrieves a specific tournament by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the tournament.</param>
    /// <returns>The tournament details if found, otherwise a 404 error.</returns>
    /// <response code="200">Returns the tournament details.</response>
    /// <response code="404">Returns not found if the tournament does not exist.</response>
    [HttpGet("get-by-id/{id:guid}")]
    [ProducesResponseType<GetTournamentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetTournamentDto>> GetTournamentById(Guid id)
    {
        var tournament = await tournamentRepo.GetByIdAsync(id);

        if (tournament == null)
            return NotFound(new ErrorResponse { Message = $"Tournament with ID {id} not found" });

        return Ok(mapper.Map<GetTournamentDto>(tournament));
    }

    /// <summary>
    /// Creates a new tournament in the system.
    /// </summary>
    /// <param name="createDto">The tournament data for creation.</param>
    /// <returns>The created tournament or validation/not found errors.</returns>
    /// <response code="201">Returns the newly created tournament.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="404">Returns not found if required dependencies are missing.</response>
    [HttpPost("create")]
    [ProducesResponseType<GetTournamentDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentDto createDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var createdTournament = await tournamentRepo.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetTournamentById), new { id = createdTournament.Id }, mapper.Map<GetTournamentDto>(createdTournament));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing tournament with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the tournament to update.</param>
    /// <param name="updateDto">The updated tournament data.</param>
    /// <returns>The updated tournament or a 404 error if not found.</returns>
    /// <response code="200">Returns the updated tournament.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="404">Returns not found if the tournament does not exist.</response>
    [HttpPut("update/{id:guid}")]
    [ProducesResponseType<GetTournamentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTournament(Guid id, [FromBody] UpdateTournamentDto updateDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updatedTournament = await tournamentRepo.UpdateAsync(id, updateDto);

            if (updatedTournament == null)
                return NotFound(new ErrorResponse { Message = $"Tournament with ID {id} not found." });

            return Ok(mapper.Map<GetTournamentDto>(updatedTournament));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a tournament from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the tournament to delete.</param>
    /// <returns>No content if successful, or 404 if not found.</returns>
    /// <response code="204">Returns no content if the tournament was successfully deleted.</response>
    /// <response code="404">Returns not found if the tournament does not exist.</response>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTournament(Guid id)
    {
        var result = await tournamentRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Tournament with ID {id} not found." });

        return NoContent();
    }
}

