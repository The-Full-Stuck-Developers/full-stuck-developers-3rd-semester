using api.Models;
using api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace tests.Services;

public class EmailServiceTests
{
    [Fact]
    public async Task SendPasswordResetEmail_SmtpNotConfigured_LogsWarning_AndReturns()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailService>>();

        var options = new AppOptions
        {
            SmtpServer = null, // triggers dev branch
            SmtpPort = 587,
            SmtpUsername = "user",
            SmtpPassword = "pass",
            SmtpFromEmail = "noreply@test.local"
        };

        var service = new EmailService(logger.Object, options);

        // Act (should NOT throw)
        await service.SendPasswordResetEmail(
            toEmail: "to@test.local",
            userName: "Test User",
            resetLink: "https://frontend.test/reset?token=abc"
        );

        // Assert: warning log happened
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("SMTP not configured") &&
                    v.ToString()!.Contains("to@test.local") &&
                    v.ToString()!.Contains("https://frontend.test/reset?token=abc")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task SendPasswordResetEmail_SmtpNotConfigured_DoesNotThrow()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailService>>();
        var options = new AppOptions { SmtpServer = "" }; // empty also triggers dev branch
        var service = new EmailService(logger.Object, options);

        // Act + Assert
        await service.SendPasswordResetEmail("to@test.local", "User", "link"); // no exception expected
    }
}