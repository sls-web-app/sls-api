using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.TournamentDto;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;

namespace sls_api.Controllers;
// summary: Kontroler API do zarządzania turniejami.
[ApiController]
[Route("api/[controller]")]
public class TournamentController(ITournamentRepo tournamentRepo, IMapper mapper) : ControllerBase
{
    // summary: Pobiera listę wszystkich turniejów.
    // returns: Lista obiektów turniejów.
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetTournamentDto>>> GetAllTournaments()
    {
        var tournaments = await tournamentRepo.GetAllAsync();
        return Ok(mapper.Map<List<GetTournamentDto>>(tournaments));
    }

    // summary: Pobiera turniej na podstawie jego identyfikatora.
    // param: id – Identyfikator turnieju.
    // returns: Szczegóły turnieju lub kod 404, jeśli nie znaleziono.
    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetTournamentDto>> GetTournamentById(Guid id)
    {
        var tournament = await tournamentRepo.GetByIdAsync(id);

        if (tournament == null)
            return NotFound(new ErrorResponse { Message = $"Tournament with ID {id} not found" });

        return Ok(mapper.Map<GetTournamentDto>(tournament));
    }

    // summary: Tworzy nowy turniej.
    // param: createDto – Dane nowego turnieju.
    // returns: Utworzony turniej lub błąd walidacji/404 w przypadku braku wymaganych danych.
    [HttpPost("create")]
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

    // summary: Aktualizuje dane istniejącego turnieju.
    // param: id – Identyfikator turnieju.
    // param: updateDto – Zaktualizowane dane turnieju.
    // returns: Zaktualizowany turniej lub kod 404, jeśli nie znaleziono.
    [HttpPut("update/{id:guid}")]
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

    // summary: Usuwa turniej.
    // param: id – Identyfikator turnieju do usunięcia.
    // returns: Kod 204 jeśli usunięto, lub 404 jeśli nie znaleziono.
    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteTournament(Guid id)
    {
        var result = await tournamentRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Tournament with ID {id} not found." });

        return NoContent();
    }
}

