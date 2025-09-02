using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.EditionDto;
using sls_borders.DTO.ErrorDto;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditionController(IEditionRepo editionRepo, IMapper mapper) : ControllerBase
    {
        //[Authorize(Roles = "Admin")]
        [HttpGet("get-all")]
        [ProducesResponseType<IEnumerable<GetEditionDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<GetEditionDto>>> GetAllEditions()
        {
            var editions = await editionRepo.GetAllAsync();
            return Ok(mapper.Map<List<GetEditionDto>>(editions));
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType<GetEditionDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetEditionDto>> GetEditionById(Guid id)
        {
            var edition = await editionRepo.GetByIdAsync(id)!;
            return Ok(mapper.Map<GetEditionDto>(edition));
        }

        [HttpGet("get-all/remaining/{teamId}")]
        [ProducesResponseType<IEnumerable<GetEditionDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<GetEditionDto>>> GetAllRemainingEditionsForTeam(Guid teamId)
        {
            var editions = await editionRepo.GetAllRemainingEditionsForTeamAsync(teamId);
            return Ok(mapper.Map<List<GetEditionDto>>(editions));
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ProducesResponseType<GetEditionDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetEditionDto>> CreateEdition([FromBody] CreateEditionDto createEditionDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var edition = mapper.Map<Edition>(createEditionDto);
                var createdEdition = await editionRepo.CreateAsync(edition);
                return CreatedAtAction(nameof(GetEditionById), new { id = createdEdition.Id }, mapper.Map<GetEditionDto>(createdEdition));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ErrorResponse { Message = ex.Message });
            }
        }
    }
}
