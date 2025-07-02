using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sls_borders.Data;
using sls_borders.DTO.UserDto;
using sls_borders.Repositories;
using AutoMapper;
using sls_borders.DTO.ErrorDto;

namespace sls_api.Controllers;

// summary: Kontroler API do zarządzania użytkownikami.
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserRepo userRepo, ApplicationDbContext dbContext, IMapper mapper) : ControllerBase
{
    // summary: Pobiera wszystkich użytkowników.
    // returns: Lista wszystkich użytkowników.
    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<GetUserDto>>> GetAllUsers()
    {
        var users = await userRepo.GetAllAsync();
        return Ok(mapper.Map<List<GetUserDto>>(users));
    }

    // summary: Pobiera użytkownika na podstawie jego identyfikatora.
    // param: id – Identyfikator użytkownika.
    // returns: Dane użytkownika lub kod 404, jeśli nie znaleziono.
    [HttpGet("get-by-id/{id:guid}")]
    public async Task<ActionResult<GetUserDto>> GetUserById(Guid id)
    {
        var user = await userRepo.GetByIdAsync(id);

        if (user == null)
            return NotFound(new ErrorResponse { Message = $"User with ID {id} not found" });

        return Ok(mapper.Map<GetUserDto>(user));
    }

    // summary: Pobiera użytkownika na podstawie adresu e-mail.
    // param: email – Adres e-mail użytkownika.
    // returns: Dane użytkownika lub kod 404, jeśli nie znaleziono.
    [HttpGet("get-by-email/{email}")]
    public async Task<ActionResult<GetUserDto>> GetUserByEmail(string email)
    {
        var user = await userRepo.GetByEmailAsync(email);

        if (user == null)
            return NotFound(new ErrorResponse { Message = $"User with email {email} not found" });

        return Ok(mapper.Map<GetUserDto>(user));
    }

    // summary: Tworzy nowego użytkownika.
    // param: dto – Dane nowego użytkownika.
    // returns: Stworzony użytkownik lub błąd walidacji/konflikt danych.
    [HttpPost("create")]
    public async Task<ActionResult<GetUserDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (await userRepo.EmailExistsAsync(dto.Email))
            return Conflict(new ErrorResponse { Message = $"User with email {dto.Email} already exists" });

        if (await dbContext.Teams.FindAsync(dto.TeamId) == null)
            return NotFound(new ErrorResponse { Message = $"Team with ID {dto.TeamId} not found" });

        try
        {
            var createdUser = await userRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, mapper.Map<GetUserDto>(createdUser));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
    }

    // summary: Aktualizuje dane użytkownika.
    // param: id – Identyfikator użytkownika.
    // param: dto – Nowe dane użytkownika.
    // returns: Zaktualizowany użytkownik lub kod 404, jeśli nie znaleziono.
    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<GetUserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updatedUser = await userRepo.UpdateAsync(id, dto);

            if (updatedUser == null)
                return NotFound(new ErrorResponse { Message = $"User with ID {id} not found" });

            return Ok(mapper.Map<GetUserDto>(updatedUser));
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
    }

    // summary: Usuwa użytkownika (tylko dla administratorów).
    // param: id – Identyfikator użytkownika.
    // returns: Kod 204 jeśli usunięto, lub 404 jeśli nie znaleziono.
    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var result = await userRepo.DeleteAsync(id);

        if (!result)
            return NotFound(new ErrorResponse { Message = $"User with ID {id} not found" });

        return NoContent();
    }

    // summary: Sprawdza, czy adres e-mail jest już używany przez innego użytkownika.
    // param: email – Adres e-mail do sprawdzenia.
    // returns: True jeśli e-mail istnieje, w przeciwnym razie false.
    [HttpGet("email-exists/{email}")]
    public async Task<ActionResult<bool>> CheckEmailExists(string email)
    {
        var exists = await userRepo.EmailExistsAsync(email);
        return Ok(exists);
    }
}

