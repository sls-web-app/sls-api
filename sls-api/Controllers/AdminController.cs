using Microsoft.AspNetCore.Mvc;
using sls_borders.DTO.Admin;
using sls_borders.Models;
using sls_borders.Repositories;

namespace sls_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(IAdminRepo adminRepo) : ControllerBase
{
    private readonly IAdminRepo _adminRepo = adminRepo;

    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetAdminDto>>> GetAllAdmins()
    {
        var admins = await _adminRepo.GetAllAsync();
        return Ok(admins);
    }

    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetAdminDto>> GetAdminById(Guid id)
    {
        var admin = await _adminRepo.GetByIdAsync(id);
        
        if (admin == null)
            return NotFound();

        return Ok(admin);
    }

    [HttpPost("create")]
    public async Task<ActionResult<GetAdminDto>> CreateAdmin([FromBody] CreateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdAdmin = await _adminRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAdminById), new { id = createdAdmin.Id }, createdAdmin);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<GetAdminDto>> UpdateAdmin(Guid id, [FromBody] UpdateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedAdmin = await _adminRepo.UpdateAsync(id, dto);

            if (updatedAdmin == null)
                return NotFound();

            return Ok(updatedAdmin);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> DeleteAdmin(Guid id)
    {
        var result = await _adminRepo.DeleteAsync(id);
        
        if (!result)
            return NotFound();

        return NoContent();
    }
}