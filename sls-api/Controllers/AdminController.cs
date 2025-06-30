using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.Admin;
using sls_borders.Repositories;
using sls_api.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sls_api.Controllers;

// summary: Kontroler API do zarządzania administratorami.
[ApiController]
[Route("api/[controller]")]
public class AdminController(IAdminRepo _adminRepo) : ControllerBase
{
    // summary: Pobiera listę wszystkich administratorów.
    // returns: Lista administratorów.
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetAdminDto>>> GetAllAdmins()
    {
        var admins = await _adminRepo.GetAllAsync();
        return Ok(admins);
    }

    // summary: Pobiera administratora na podstawie identyfikatora.
    // param: id – Unikalny identyfikator administratora.
    // returns: Administrator lub kod 404, jeśli nie znaleziono.
    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetAdminDto>> GetAdminById(Guid id)
    {
        var admin = await _adminRepo.GetByIdAsync(id);

        if (admin == null)
            return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

        return Ok(admin);
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
            var createdAdmin = await _adminRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAdminById), new { id = createdAdmin.Id }, createdAdmin);
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
            var updatedAdmin = await _adminRepo.UpdateAsync(id, dto);

            if (updatedAdmin == null)
                return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

            return Ok(updatedAdmin);
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
        var result = await _adminRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"Admin with ID {id} not found" });

        return NoContent();
    }
}
