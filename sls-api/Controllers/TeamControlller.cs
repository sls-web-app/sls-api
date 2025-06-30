using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.Team;
using sls_borders.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sls_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepo _teamRepo;

        public TeamController(ITeamRepo teamRepo)
        {
            _teamRepo = teamRepo;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<GetTeamDto>>> GetAllTeams()
        {
            var teams = await _teamRepo.GetAllAsync();
            return Ok(teams);
        }

        [HttpGet("get-by-id/{id:guid}")]
        public async Task<ActionResult<GetTeamDto>> GetTeamById(Guid id)
        {
            var team = await _teamRepo.GetByIdAsync(id);

            if (team == null)
            {
                return NotFound($"Team with ID {id} not found");
            }

            return Ok(team);
        }
        
        [HttpGet("get-tournaments/{id:guid}")]
        public async Task<ActionResult<GetTeamTournamentsDto>> GetTeamTournaments(Guid id)
        {
            try
            {
                var tournaments = await _teamRepo.GetTeamTournamentsInfo(id);
                return Ok(tournaments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var createdTeam = await _teamRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetTeamById), new { id = createdTeam.Id }, createdTeam);
        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] UpdateTeamDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updatedTeam = await _teamRepo.UpdateAsync(id, updateDto);

            if (updatedTeam == null)
            {
                return NotFound($"Team with ID {id} not found.");
            }

            return Ok(updatedTeam);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            var result = await _teamRepo.DeleteAsync(id);

            if (!result)
            {
                return NotFound($"Team with ID {id} not found.");
            }

            return NoContent();
        }
    }
}