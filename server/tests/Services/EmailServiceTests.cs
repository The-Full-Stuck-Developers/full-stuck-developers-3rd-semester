using api.Models;
using api.Services;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using Xunit;

namespace api.Tests.Services;

public class EmailServiceTests
{
    private readonly TestLogger<EmailService> _logger;

    public EmailServiceTests()
    {
        _logger = new TestLogger<EmailService>();
    }

    #region Development mode (No SMTP)

    [Fact]
    public async Task SendPasswordResetEmail_LogsWarning_WhenSmtpNotConfigured()
    {
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);

        var toEmail = "test@example.com";
        var userName = "Test User";
        var resetLink = "https://example.com/reset?token=abc123";

        await emailService.SendPasswordResetEmail(toEmail, userName, resetLink);

        Assert.True(_logger.WarningLogged);
        Assert.Contains(_logger.LoggedMessages,
            msg => msg.Contains("SMTP not configured") &&
                   msg.Contains(toEmail) &&
                   msg.Contains(resetLink));
    }

    [Fact]
    public async Task SendPasswordResetEmail_LogsWarning_WhenSmtpServerIsEmpty()
    {
        var appOptions = new AppOptions { SmtpServer = "" };
        var emailService = new EmailService(_logger, appOptions);

        await emailService.SendPasswordResetEmail(
            "user@test.com",
            "John Doe",
            "https://app.com/reset?token=xyz789");

        Assert.True(_logger.WarningLogged);
        Assert.Contains(_logger.LoggedMessages, msg => msg.Contains("SMTP not configured"));
    }

    [Fact]
    public async Task SendPasswordResetEmail_DoesNotThrow_WhenSmtpNotConfigured()
    {
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);

        await emailService.SendPasswordResetEmail(
            "test@example.com",
            "Test User",
            "https://example.com/reset");
    }

    [Fact]
    public async Task SendPasswordResetEmail_IncludesAllParameters_InLogMessage()
    {
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);

        var email = "specific@email.com";
        var resetLink = "https://myapp.com/reset?token=unique123";

        await emailService.SendPasswordResetEmail(email, "User Name", resetLink);

        var logMessage = _logger.LoggedMessages.FirstOrDefault();
        Assert.NotNull(logMessage);
        Assert.Contains(email, logMessage!);
        Assert.Contains(resetLink, logMessage!);
    }

    #endregion

    #region SMTP configured (network-dependent)

    [Fact]
    public async Task SendPasswordResetEmail_WhenSmtpConfigured_ThrowsSomeException_IfServerUnreachable()
    {
        // NOTE: This is still not a perfect unit test, but it is stable:
        // we assert "throws" without depending on exact exception type.
        var appOptions = new AppOptions
        {
            SmtpServer = "invalid-smtp-server.local",
            SmtpPort = 587,
            SmtpUsername = "user@example.com",
            SmtpPassword = "password",
            SmtpFromEmail = "noreply@example.com"
        };

        var emailService = new EmailService(_logger, appOptions);

        await Assert.ThrowsAnyAsync<Exception>(() =>
            emailService.SendPasswordResetEmail(
                "test@example.com",
                "Test User",
                "https://example.com/reset"));
    }

    #endregion

    #region Edge cases

    [Fact]
    public async Task SendPasswordResetEmail_HandlesSpecialCharacters_InUserName()
    {
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);
        var userName = "Test <User> & \"Special\" 'Characters'";

        await emailService.SendPasswordResetEmail(
            "test@example.com",
            userName,
            "https://example.com/reset");

        Assert.True(_logger.WarningLogged);
    }

    [Fact]
    public async Task SendPasswordResetEmail_HandlesLongResetLink()
    {
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);

        var longToken = new string('a', 500);
        var resetLink = $"https://example.com/reset?token={longToken}";

        await emailService.SendPasswordResetEmail(
            "test@example.com",
            "Test User",
            resetLink);

        Assert.Contains(_logger.LoggedMessages, msg => msg.Contains(resetLink));
    }

    [Fact]
    public async Task SendPasswordResetEmail_HandlesEmptyUserName()
    {
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);

        await emailService.SendPasswordResetEmail(
            "test@example.com",
            "",
            "https://example.com/reset");

        Assert.True(_logger.WarningLogged);
    }

    #endregion
}

// Test logger implementation to capture log messages
public class TestLogger<T> : ILogger<T>
{
    public List<string> LoggedMessages { get; } = new();
    public bool WarningLogged { get; private set; }
    public bool InformationLogged { get; private set; }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        LoggedMessages.Add(message);

        if (logLevel == LogLevel.Warning) WarningLogged = true;
        if (logLevel == LogLevel.Information) InformationLogged = true;
    }
}
