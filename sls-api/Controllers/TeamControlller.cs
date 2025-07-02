using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.Team;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;

namespace sls_api.Controllers;
// summary: Kontroler API do zarządzania drużynami.
[ApiController]
[Route("api/[controller]")]
public class TeamController(ITeamRepo teamRepo, IMapper mapper) : ControllerBase
{
    // summary: Pobiera wszystkie drużyny.
    // returns: Lista wszystkich drużyn.
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetTeamDto>>> GetAllTeams()
    {
        var teams = await teamRepo.GetAllAsync();
        return Ok(mapper.Map<List<GetTeamDto>>(teams));
    }

    // summary: Pobiera drużynę na podstawie jej identyfikatora.
    // param: id – Identyfikator drużyny.
    // returns: Szczegóły drużyny lub kod 404, jeśli nie znaleziono.
    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetTeamDto>> GetTeamById(Guid id)
    {
        var team = await teamRepo.GetByIdAsync(id);

        if (team == null)
            return NotFound(new ErrorResponse { Message = $"Team with ID {id} not found" });

        return Ok(mapper.Map<GetTeamDto>(team));
    }

    // summary: Pobiera informacje o turniejach, w których brała udział dana drużyna.
    // param: id – Identyfikator drużyny.
    // returns: Lista turniejów lub kod 404, jeśli drużyna nie istnieje.
    [HttpGet("get-tournaments/{id:guid}")]
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

    // summary: Tworzy nową drużynę.
    // param: dto – Dane nowej drużyny.
    // returns: Utworzona drużyna lub błąd walidacji.
    [HttpPost("create")]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var createdTeam = await teamRepo.CreateAsync(dto);
        return CreatedAtAction(nameof(GetTeamById), new { id = createdTeam.Id }, mapper.Map<GetTeamDto>(createdTeam));
    }

    // summary: Aktualizuje dane istniejącej drużyny.
    // param: id – Identyfikator drużyny.
    // param: updateDto – Nowe dane drużyny.
    // returns: Zaktualizowana drużyna lub kod 404, jeśli nie znaleziono.
    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] UpdateTeamDto updateDto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var updatedTeam = await teamRepo.UpdateAsync(id, updateDto);

        if (updatedTeam == null)
            return NotFound(new ErrorResponse { Message = $"Team with ID {id} not found." });

        return Ok(mapper.Map<GetTeamDto>(updatedTeam));
    }

    // summary: Usuwa drużynę.
    // param: id – Identyfikator drużyny.
    // returns: Kod 204 jeśli usunięto, lub 404 jeśli nie znaleziono.
    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var result = await teamRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Team with ID {id} not found." });

        return NoContent();
    }
}


