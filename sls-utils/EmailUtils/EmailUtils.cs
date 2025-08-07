using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace sls_utils.EmailUtils;

/// <summary>
/// Class EmailUtils provides methods for sending emails using SMTP.
/// </summary>
public class EmailUtils
{
    /// <summary>
    /// Sends an email asynchronously using the specified configuration, recipient, subject, and body.
    /// </summary>
    /// <param name="emailConfig">The email configuration section containing SMTP settings.</param>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body content of the email.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SendEmailAsync(IConfigurationSection emailConfig, string toEmail, string subject, string body)
    {
        if (emailConfig["SenderEmail"] == null || emailConfig["SenderName"] == null || emailConfig["Server"] == null || emailConfig["Port"] == null || emailConfig["Password"] == null)
        {
            throw new ArgumentException("Email configuration is incomplete.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailConfig["SenderName"], emailConfig["SenderEmail"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(emailConfig["Server"], int.Parse(emailConfig["Port"]!), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(emailConfig["SenderEmail"], emailConfig["Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    /// <summary>
    /// Sends a registration email asynchronously to the specified recipient with a password setup URL.
    /// </summary>
    /// <param name="emailConfig">The email configuration section containing SMTP settings.</param>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="userName">The name of the user to include in the email.</param>
    /// <param name="PasswordSetupUrl">The URL for the user to set up their password.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SendRegisterEmailAsync(IConfigurationSection emailConfig, string toEmail, string userName, string PasswordSetupUrl)
    {
        string subject = "SLS Registration";
        string body = await File.ReadAllTextAsync("Templates/registration_email.html");
        body = body.Replace("{{UserName}}", userName).Replace("{{PasswordSetupUrl}}", PasswordSetupUrl);
        await SendEmailAsync(emailConfig, toEmail, subject, body);
    }
}