using System.Net;
using System.Net.Mail;
using api.Models;
using api.Services;

namespace api.Services;
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly AppOptions _appOptions;
    
    public EmailService(ILogger<EmailService> logger, AppOptions appOptions)
    {
        _logger = logger;
        _appOptions = appOptions;
    }
    
    public async Task SendPasswordResetEmail(string toEmail, string userName, string resetLink)
    {
        // For development, just log the link
        if (string.IsNullOrEmpty(_appOptions.SmtpServer))
        {
            _logger.LogWarning("SMTP not configured. Password reset link for {Email}: {Link}", toEmail, resetLink);
            return;
        }
        
        // Production: Use MailKit or similar
        using var client = new SmtpClient(_appOptions.SmtpServer, _appOptions.SmtpPort);
        client.Credentials = new NetworkCredential(_appOptions.SmtpUsername, _appOptions.SmtpPassword);
        client.EnableSsl = true;
        
        var message = new MailMessage
        {
            From = new MailAddress(_appOptions.SmtpFromEmail, "Your App Name"),
            Subject = "Password Reset Request",
            Body = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hi {userName},</p>
                    <p>You requested to reset your password. Click the link below to reset it:</p>
                    <p><a href=""{resetLink}"">Reset Password</a></p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                </body>
                </html>
            ",
            IsBodyHtml = true
        };
        message.To.Add(toEmail);
        
        await client.SendMailAsync(message);
        _logger.LogInformation("Password reset email sent to {Email}", toEmail);
    }
}