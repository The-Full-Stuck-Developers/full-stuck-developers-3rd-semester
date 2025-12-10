namespace api.Services;

public interface IEmailService
{
    Task SendPasswordResetEmail(string toEmail, string userName, string resetLink);

}