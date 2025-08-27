using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.GameDto;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;

namespace sls_api.Controllers;

/// <summary>
/// API controller for game/match management.
/// Provides CRUD operations for games including creation, retrieval, updates, and deletion.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GameController(IGameRepo gameRepo, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Retrieves all games from the system.
    /// </summary>
    /// <returns>A collection of all games.</returns>
    /// <response code="200">Returns the list of all games.</response>
    [HttpGet("get-all")]
    [ProducesResponseType<IEnumerable<GetGameDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetGameDto>>> GetAllGames()
    {
        var games = await gameRepo.GetAllAsync();
        return Ok(mapper.Map<List<GetGameDto>>(games));
    }

    /// <summary>
    /// Retrieves a specific game by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the game.</param>
    /// <returns>The game details if found.</returns>
    /// <response code="200">Returns the game details.</response>
    /// <response code="404">Returns not found if the game does not exist.</response>
    [HttpGet("get-by-id/{id:guid}")]
    [ProducesResponseType<GetGameDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetGameDto>> GetGameById(Guid id)
    {
        var game = await gameRepo.GetByIdAsync(id)!;

        return Ok(mapper.Map<GetGameDto>(game));
    }

    /// <summary>
    /// Creates a new game in the system.
    /// </summary>
    /// <param name="dto">The game data for creation.</param>
    /// <returns>The created game or validation/conflict errors.</returns>
    /// <response code="201">Returns the newly created game.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="409">Returns conflict if there's a business logic violation.</response>
    [HttpPost("create")]
    [ProducesResponseType<GetGameDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetGameDto>> CreateGame([FromBody] CreateGameDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var createdGame = await gameRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, mapper.Map<GetGameDto>(createdGame));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing game with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the game to update.</param>
    /// <param name="dto">The updated game data.</param>
    /// <returns>The updated game or a 404 error if not found.</returns>
    /// <response code="200">Returns the updated game.</response>
    /// <response code="400">Returns validation errors if the request model is invalid.</response>
    /// <response code="404">Returns not found if the game does not exist.</response>
    /// <response code="409">Returns conflict if there's a business logic violation.</response>
    [HttpPut("update/{id:guid}")]
    [ProducesResponseType<GetGameDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetGameDto>> UpdateGame(Guid id, [FromBody] UpdateGameDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updatedGame = await gameRepo.UpdateAsync(id, dto);

            if (updatedGame == null)
                return NotFound(new ErrorResponse { Message = $"Game with ID {id} not found" });

            return Ok(mapper.Map<GetGameDto>(updatedGame));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a game from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the game to delete.</param>
    /// <returns>No content if successful, or 404 if not found.</returns>
    /// <response code="204">Returns no content if the game was successfully deleted.</response>
    /// <response code="404">Returns not found if the game does not exist.</response>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGame(Guid id)
    {
        var result = await gameRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Game with ID {id} not found" });

        return NoContent();
    }
}

