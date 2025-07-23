namespace sls_borders.Repositories;

public interface IEmailRepo
{
    public Task SendEmailAsync(string toEmail, string subject, string body);
}