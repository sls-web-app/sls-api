using System.ComponentModel.DataAnnotations;

namespace sls_api.DTO;

/// <summary>
/// DTO dla ��dania tworzenia nowego zespo�u z obs�ug� form-data.
/// </summary>
public class CreateTeamRequestDto
{
    /// <summary>
    /// Nazwa zespo�u.
    /// </summary>
    [Required(ErrorMessage = "Nazwa zespo�u jest wymagana")]
    [StringLength(100, ErrorMessage = "Nazwa zespo�u nie mo�e by� d�u�sza ni� 100 znak�w")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Skr�cona nazwa zespo�u.
    /// </summary>
    [Required(ErrorMessage = "Skr�cona nazwa zespo�u jest wymagana")]
    [StringLength(20, ErrorMessage = "Skr�cona nazwa zespo�u nie mo�e by� d�u�sza ni� 20 znak�w")]
    public string Short { get; set; } = null!;

    /// <summary>
    /// Adres zespo�u.
    /// </summary>
    [Required(ErrorMessage = "Adres zespo�u jest wymagany")]
    [StringLength(200, ErrorMessage = "Adres zespo�u nie mo�e by� d�u�szy ni� 200 znak�w")]
    public string Address { get; set; } = null!;

    /// <summary>
    /// Plik obrazu zespo�u.
    /// </summary>
    public IFormFile? Image { get; set; }
}