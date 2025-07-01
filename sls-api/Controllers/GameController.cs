using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.Game;
using sls_borders.Repositories;
using sls_api.Utils;
using AutoMapper;

namespace sls_api.Controllers;

// summary: Kontroler API do zarządzania meczami.
[ApiController]
[Route("api/[controller]")]
public class GameController(IGameRepo gameRepo, IMapper mapper) : ControllerBase
{
    // summary: Pobiera wszystkie mecze.
    // returns: Lista wszystkich meczów.
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetGameDto>>> GetAllGames()
    {
        var games = await gameRepo.GetAllAsync();
        return Ok(mapper.Map<List<GetGameDto>>(games));
    }

    // summary: Pobiera mecz na podstawie jego identyfikatora.
    // param: id – Identyfikator meczu.
    // returns: Szczegóły meczu lub kod 404, jeśli nie znaleziono.
    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetGameDto>> GetGameById(Guid id)
    {
        var game = await gameRepo.GetByIdAsync(id)!;

        return Ok(mapper.Map<GetGameDto>(game));
    }

    // summary: Tworzy nowy mecz.
    // param: dto – Dane nowego meczu.
    // returns: Utworzony mecz lub błąd walidacji/konflikt.
    [HttpPost("create")]
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

    // summary: Aktualizuje dane meczu.
    // param: id – Identyfikator meczu.
    // param: dto – Zaktualizowane dane meczu.
    // returns: Zaktualizowany mecz lub kod 404, jeśli nie znaleziono.
    [HttpPut("update/{id:guid}")]
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

    // summary: Usuwa mecz.
    // param: id – Identyfikator meczu.
    // returns: Kod 204 jeśli usunięto, lub kod 404 jeśli nie znaleziono.
    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> DeleteGame(Guid id)
    {
        var result = await gameRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Game with ID {id} not found" });

        return NoContent();
    }
}

