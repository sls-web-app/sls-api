using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using sls_borders.Repositories;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace sls_repos.Repositories;

public class EmailRepo : IEmailRepo
{
    private readonly IConfiguration _config;

    public EmailRepo(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _config.GetSection("SmtpSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(emailSettings["Server"], int.Parse(emailSettings["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}