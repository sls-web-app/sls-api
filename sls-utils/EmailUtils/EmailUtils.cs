using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace sls_utils.EmailUtils;

public class EmailUtils
{
    public static async Task SendEmailAsync(IConfigurationSection emailConfig , string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailConfig["SenderName"], emailConfig["SenderEmail"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(emailConfig["Server"], int.Parse(emailConfig["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailConfig["SenderEmail"], emailConfig["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}