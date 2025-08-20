using System.ComponentModel.DataAnnotations;

namespace sls_api.DTO;

/// <summary>
/// DTO dla ¿¹dania tworzenia nowego zespo³u z obs³ug¹ form-data.
/// </summary>
public class CreateTeamRequestDto
{
    /// <summary>
    /// Nazwa zespo³u.
    /// </summary>
    [Required(ErrorMessage = "Nazwa zespo³u jest wymagana")]
    [StringLength(100, ErrorMessage = "Nazwa zespo³u nie mo¿e byæ d³u¿sza ni¿ 100 znaków")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Skrócona nazwa zespo³u.
    /// </summary>
    [Required(ErrorMessage = "Skrócona nazwa zespo³u jest wymagana")]
    [StringLength(20, ErrorMessage = "Skrócona nazwa zespo³u nie mo¿e byæ d³u¿sza ni¿ 20 znaków")]
    public string Short { get; set; } = null!;

    /// <summary>
    /// Adres zespo³u.
    /// </summary>
    [Required(ErrorMessage = "Adres zespo³u jest wymagany")]
    [StringLength(200, ErrorMessage = "Adres zespo³u nie mo¿e byæ d³u¿szy ni¿ 200 znaków")]
    public string Address { get; set; } = null!;

    /// <summary>
    /// Plik obrazu zespo³u.
    /// </summary>
    public IFormFile? Image { get; set; }
}