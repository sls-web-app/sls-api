using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.Game;
using sls_borders.Repositories;

namespace sls_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(IGameRepo gameRepo) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetGameDto>>> GetAllGames()
    {
        var games = await gameRepo.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetGameDto>> GetGameById(Guid id)
    {
        var game = await gameRepo.GetByIdAsync(id);
        
        if (game == null)
            return NotFound();

        return Ok(game);
    }

    [HttpPost("create")]
    public async Task<ActionResult<GetGameDto>> CreateGame([FromBody] CreateGameDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdGame = await gameRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<GetGameDto>> UpdateGame(Guid id, [FromBody] UpdateGameDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedGame = await gameRepo.UpdateAsync(id, dto);

            if (updatedGame == null)
                return NotFound();

            return Ok(updatedGame);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> DeleteGame(Guid id)
    {
        var result = await gameRepo.DeleteAsync(id);
        
        if (!result)
            return NotFound();

        return NoContent();
    }
}