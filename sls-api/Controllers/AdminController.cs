using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.Admin;
using sls_borders.Repositories;
using sls_api.Utils;
using AutoMapper;

namespace sls_api.Controllers;

// summary: Kontroler API do zarządzania administratorami.
[ApiController]
[Route("api/[controller]")]
public class AdminController(IAdminRepo adminRepo, IMapper mapper) : ControllerBase
{
    // summary: Pobiera listę wszystkich administratorów.
    // returns: Lista administratorów.
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetAdminDto>>> GetAllAdmins()
    {
        var admins = await adminRepo.GetAllAsync();
        var adminDtos = mapper.Map<List<GetAdminDto>>(admins);
        return Ok(adminDtos);
    }

    // summary: Pobiera administratora na podstawie identyfikatora.
    // param: id – Unikalny identyfikator administratora.
    // returns: Administrator lub kod 404, jeśli nie znaleziono.
    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetAdminDto>> GetAdminById(Guid id)
    {
        var admin = await adminRepo.GetByIdAsync(id);

        if (admin == null)
            return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

        var dto = mapper.Map<GetAdminDto>(admin);
        return Ok(dto);
    }

    // summary: Tworzy nowego administratora.
    // param: dto – Dane nowego administratora.
    // returns: Utworzony administrator lub kod błędu walidacji/konfliktu.
    [HttpPost("create")]
    public async Task<ActionResult<GetAdminDto>> CreateAdmin([FromBody] CreateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var createdAdmin = await adminRepo.CreateAsync(dto);
            var responseDto = mapper.Map<GetAdminDto>(createdAdmin);
            return CreatedAtAction(nameof(GetAdminById), new { id = responseDto.Id }, responseDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    // summary: Aktualizuje dane administratora.
    // param: id – Unikalny identyfikator administratora.
    // param: dto – Nowe dane administratora.
    // returns: Zaktualizowany administrator lub kod błędu/404.
    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<GetAdminDto>> UpdateAdmin(Guid id, [FromBody] UpdateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updatedAdmin = await adminRepo.UpdateAsync(id, dto);

            if (updatedAdmin == null)
                return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

            var responseDto = mapper.Map<GetAdminDto>(updatedAdmin);
            return Ok(responseDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    // summary: Usuwa administratora.
    // param: id – Unikalny identyfikator administratora.
    // returns: Kod 204 jeśli usunięto, lub kod 404 jeśli nie znaleziono.
    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> DeleteAdmin(Guid id)
    {
        var result = await adminRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

        return NoContent();
    }
}

