using Microsoft.AspNetCore.Mvc;
using sls_borders.Repositories;

namespace sls_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailRepo _emailRepo;

    public EmailController(IEmailRepo emailRepo)
    {
        _emailRepo = emailRepo;
    }

    /// <summary>
    /// Sends an email using the configured SMTP settings.
    /// </summary>
    /// <param name="request">The email request containing recipient, subject, and body.</param>
    /// <returns>Action result indicating success or failure.</returns>
    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ToEmail))
        {
            return BadRequest("ToEmail, Subject, and Body are required.");
        }

        try
        {
            await _emailRepo.SendEmailAsync(request.ToEmail, "Potwierdź swój adres e-mail",
                @"
                <div style='font-family: Arial, sans-serif; background: #f9f9f9; padding: 30px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); padding: 32px;'>
                    
                        <h2 style='color: #2d7ff9; text-align: center;'>Witamy w Słupskiej Lidze Szachowej!</h2>
                        <p style='font-size: 16px; color: #333; text-align: center;'>
                            Dziękujemy za rejestrację.<br>
                            Prosimy potwierdzić swój adres e-mail, aby aktywować konto.
                        </p>
                        <div style='text-align: center; margin: 32px 0;'>
                            <a href='' 
                            style='background: #2d7ff9; color: #fff; text-decoration: none; padding: 14px 28px; border-radius: 5px; font-size: 16px; display: inline-block;'>
                                Potwierdź e-mail
                            </a>
                        </div>
                        <p style='font-size: 13px; color: #888; text-align: center;'>
                            Jeśli nie zakładałeś konta, możesz zignorować tę wiadomość.
                        </p>
                    </div>
                </div>
                ");
            return Ok("Email sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to send email: {ex.Message}");
        }
    }
}

public class SendEmailRequest
{
    public string ToEmail { get; set; } = string.Empty;
}