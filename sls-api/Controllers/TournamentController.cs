using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.TournamentDto;
using sls_borders.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sls_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentController(ITournamentRepo tournamentRepo) : ControllerBase
    {
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<GetTournamentDto>>> GetAllTournaments()
        {
            var tournaments = await tournamentRepo.GetAllAsync();
            return Ok(tournaments);
        }

        [HttpGet("get-by-id/{id:guid}")]
        public async Task<ActionResult<GetTournamentDto>> GetTournamentById(Guid id)
        {
            var tournament = await tournamentRepo.GetByIdAsync(id);

            if (tournament == null)
            {
                return NotFound($"Tournament with ID {id} not found");
            }

            return Ok(tournament);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var createdTournament = await tournamentRepo.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetTournamentById), new { id = createdTournament.Id }, createdTournament);
            }
            catch (KeyNotFoundException ex)
            {
                // Return a 404 Not Found if the organizing team doesn't exist.
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateTournament(Guid id, [FromBody] UpdateTournamentDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var updatedTournament = await tournamentRepo.UpdateAsync(id, updateDto);

                if (updatedTournament == null)
                {
                    return NotFound($"Tournament with ID {id} not found.");
                }

                return Ok(updatedTournament);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteTournament(Guid id)
        {
            var result = await tournamentRepo.DeleteAsync(id);

            if (!result)
            {
                return NotFound($"Tournament with ID {id} not found.");
            }

            return NoContent();
        }
    }
}