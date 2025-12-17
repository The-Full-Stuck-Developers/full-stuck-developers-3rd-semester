using api.Models;
using api.Services;
using Microsoft.Extensions.Logging;

namespace api.Tests.Services;

public class EmailServiceTests
{
    private readonly TestLogger<EmailService> _logger;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _logger = new TestLogger<EmailService>();
    }

    #region SendPasswordResetEmail Tests - Development Mode (No SMTP)

    [Fact]
    public async Task SendPasswordResetEmail_LogsWarning_WhenSmtpNotConfigured()
    {
        // Arrange
        var appOptions = new AppOptions
        {
            SmtpServer = null // No SMTP configured
        };
        var emailService = new EmailService(_logger, appOptions);
        var toEmail = "test@example.com";
        var userName = "Test User";
        var resetLink = "https://example.com/reset?token=abc123";

        // Act
        await emailService.SendPasswordResetEmail(toEmail, userName, resetLink);

        // Assert
        Assert.Contains(_logger.LoggedMessages, 
            msg => msg.Contains("SMTP not configured") && 
                   msg.Contains(toEmail) && 
                   msg.Contains(resetLink));
        Assert.True(_logger.WarningLogged);
    }

    [Fact]
    public async Task SendPasswordResetEmail_LogsWarning_WhenSmtpServerIsEmpty()
    {
        // Arrange
        var appOptions = new AppOptions
        {
            SmtpServer = "" // Empty SMTP server
        };
        var emailService = new EmailService(_logger, appOptions);
        var toEmail = "user@test.com";
        var userName = "John Doe";
        var resetLink = "https://app.com/reset?token=xyz789";

        // Act
        await emailService.SendPasswordResetEmail(toEmail, userName, resetLink);

        // Assert
        Assert.Contains(_logger.LoggedMessages, 
            msg => msg.Contains("SMTP not configured"));
        Assert.True(_logger.WarningLogged);
    }

    [Fact]
    public async Task SendPasswordResetEmail_DoesNotThrowException_WhenSmtpNotConfigured()
    {
        // Arrange
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);

        // Act & Assert - Should not throw
        await emailService.SendPasswordResetEmail(
            "test@example.com", 
            "Test User", 
            "https://example.com/reset");
    }

    [Fact]
    public async Task SendPasswordResetEmail_IncludesAllParameters_InLogMessage()
    {
        // Arrange
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);
        var email = "specific@email.com";
        var resetLink = "https://myapp.com/reset?token=unique123";

        // Act
        await emailService.SendPasswordResetEmail(email, "User Name", resetLink);

        // Assert
        var logMessage = _logger.LoggedMessages.FirstOrDefault();
        Assert.NotNull(logMessage);
        Assert.Contains(email, logMessage);
        Assert.Contains(resetLink, logMessage);
    }

    #endregion

    #region SendPasswordResetEmail Tests - Production Mode (With SMTP)

    [Fact]
    public async Task SendPasswordResetEmail_ThrowsException_WhenSmtpServerIsInvalid()
    {
        // Arrange
        var appOptions = new AppOptions
        {
            SmtpServer = "invalid-smtp-server.local",
            SmtpPort = 587,
            SmtpUsername = "user@example.com",
            SmtpPassword = "password",
            SmtpFromEmail = "noreply@example.com"
        };
        var emailService = new EmailService(_logger, appOptions);

        // Act & Assert
        // This will throw an exception because the SMTP server is invalid
        await Assert.ThrowsAsync<System.Net.Sockets.SocketException>(async () =>
            await emailService.SendPasswordResetEmail(
                "test@example.com",
                "Test User",
                "https://example.com/reset"));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task SendPasswordResetEmail_HandlesSpecialCharacters_InUserName()
    {
        // Arrange
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);
        var userName = "Test <User> & \"Special\" 'Characters'";

        // Act
        await emailService.SendPasswordResetEmail(
            "test@example.com",
            userName,
            "https://example.com/reset");

        // Assert - Should not throw
        Assert.True(_logger.WarningLogged);
    }

    [Fact]
    public async Task SendPasswordResetEmail_HandlesLongResetLink()
    {
        // Arrange
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);
        var longToken = new string('a', 500);
        var resetLink = $"https://example.com/reset?token={longToken}";

        // Act
        await emailService.SendPasswordResetEmail(
            "test@example.com",
            "Test User",
            resetLink);

        // Assert
        Assert.Contains(_logger.LoggedMessages, msg => msg.Contains(resetLink));
    }

    [Fact]
    public async Task SendPasswordResetEmail_HandlesEmptyUserName()
    {
        // Arrange
        var appOptions = new AppOptions { SmtpServer = null };
        var emailService = new EmailService(_logger, appOptions);

        // Act
        await emailService.SendPasswordResetEmail(
            "test@example.com",
            "",
            "https://example.com/reset");

        // Assert - Should not throw
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

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        LoggedMessages.Add(message);

        if (logLevel == LogLevel.Warning)
        {
            WarningLogged = true;
        }
        else if (logLevel == LogLevel.Information)
        {
            InformationLogged = true;
        }
    }
}