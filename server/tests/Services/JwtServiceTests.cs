using api.Models.Dtos.Responses;
using api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;

namespace tests.Services;

public class JwtServiceTests
{
    private static IConfiguration BuildConfigWithSecret(byte[] rawKey)
    {
        var base64 = Convert.ToBase64String(rawKey);

        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppOptions:JwtSecret"] = base64
            })
            .Build();
    }

    private static AuthUserInfo NewUser(bool isAdmin = false)
        => new AuthUserInfo(
            Guid.NewGuid(),
            "Test User",
            isAdmin,
            ExpiresAt: null,
            DeletedAt: null
        );

    [Fact]
    public void CreateToken_ReturnsJwt_ThatValidates_WithValidationParams()
    {
        var cfg = BuildConfigWithSecret(RandomBytes(64));
        var sut = new JwtService(cfg);
        var user = NewUser(isAdmin: true);

        var token = sut.CreateToken(user);

        Assert.False(string.IsNullOrWhiteSpace(token));

        var handler = new JsonWebTokenHandler();
        var validationParams = JwtService.CreateValidationParams(cfg);

        var result = handler.ValidateToken(token, validationParams);

        Assert.True(result.IsValid, result.Exception?.ToString());
        Assert.NotNull(result.ClaimsIdentity);
        Assert.True(result.ClaimsIdentity!.IsAuthenticated);
    }

    [Fact]
    public void CreateToken_ContainsUserIdClaim()
    {
        var cfg = BuildConfigWithSecret(RandomBytes(64));
        var sut = new JwtService(cfg);

        var userId = Guid.NewGuid();
        var user = new AuthUserInfo(
            userId,
            "Alice",
            true,
            ExpiresAt: null,
            DeletedAt: null
        );

        var token = sut.CreateToken(user);

        var jwt = new JsonWebToken(token);

        var hasUserId = false;
        foreach (var c in jwt.Claims)
        {
            if ((c.Type.EndsWith("/nameidentifier", StringComparison.OrdinalIgnoreCase) ||
                 c.Type.Equals("sub", StringComparison.OrdinalIgnoreCase) ||
                 c.Type.Equals("nameid", StringComparison.OrdinalIgnoreCase) ||
                 c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                     StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(c.Value, userId.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                hasUserId = true;
                break;
            }
        }

        Assert.True(hasUserId, "JWT did not contain the user's id claim. Check AuthUserInfo.ToClaims().");
    }

    [Fact]
    public void CreateValidationParams_Throws_WhenSecretMissing()
    {
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        Assert.ThrowsAny<Exception>(() => JwtService.CreateValidationParams(cfg));
    }

    [Fact]
    public void TokenSignedWithDifferentSecret_DoesNotValidate()
    {
        var cfg1 = BuildConfigWithSecret(RandomBytes(64));
        var cfg2 = BuildConfigWithSecret(RandomBytes(64)); // different key

        var sut = new JwtService(cfg1);
        var token = sut.CreateToken(NewUser());

        var handler = new JsonWebTokenHandler();
        var wrongParams = JwtService.CreateValidationParams(cfg2);

        var result = handler.ValidateToken(token, wrongParams);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateToken_ExpiresAboutSevenDaysFromNow()
    {
        var cfg = BuildConfigWithSecret(RandomBytes(64));
        var sut = new JwtService(cfg);

        var token = sut.CreateToken(NewUser());

        var jwt = new JsonWebToken(token);
        var now = DateTime.UtcNow;

        Assert.True(jwt.ValidTo > now.AddDays(6.9));
        Assert.True(jwt.ValidTo < now.AddDays(7.1));
    }

    private static byte[] RandomBytes(int len)
    {
        var b = new byte[len];
        System.Security.Cryptography.RandomNumberGenerator.Fill(b);
        return b;
    }
}
