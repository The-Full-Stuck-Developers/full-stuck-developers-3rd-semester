using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using api.Models;
using api.Models.Requests;
using dataccess;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using ValidationException = Bogus.ValidationException;

namespace api.Services;

public class AuthService(
    MyDbContext ctx,
    ILogger<AuthService> logger,
    TimeProvider timeProvider,
    AppOptions appOptions) : IAuthService
{
    public async Task<JwtClaims> VerifyAndDecodeToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ValidationException("No token attached!");

        var builder = CreateJwtBuilder();

        string jsonString;
        try
        {
            jsonString = builder.Decode(token)
                         ?? throw new ValidationException("Authentication failed!");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message, e);
            throw new ValidationException("Valided to verify JWT");
        }

        var jwtClaims = JsonSerializer.Deserialize<JwtClaims>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new ValidationException("Authentication failed!");

        _ = ctx.Users.FirstOrDefault(u => u.Id == jwtClaims.Id)
            ?? throw new ValidationException("Authentication is valid, but user is not found!");

        return jwtClaims;
    }

    public async Task<JwtResponse> Login(LoginRequestDto dto)
    {
        var user = ctx.Users.FirstOrDefault(u => u.Email == dto.Email)
                   ?? throw new ValidationException("User is not found!");
        var passwordsMatch = user.Password ==
                             SHA512.HashData(
                                     Encoding.UTF8.GetBytes(dto.Password))
                                 .Aggregate("", (current, b) => current + b.ToString("x2"));
        if (!passwordsMatch)
            throw new ValidationException("Password is incorrect!");

        var token = CreateJwt(user);
        return new JwtResponse(token);
    }

    public async Task<JwtResponse> Register(RegisterRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var isEmailTaken = ctx.Users.Any(u => u.Email == dto.Email);
        if (isEmailTaken)
            throw new ValidationException("Email is already taken");

        var salt = Guid.NewGuid().ToString();
        var hash = SHA512.HashData(
            Encoding.UTF8.GetBytes(dto.Password + salt));
        var user = new User
        {
            Email = dto.Email,
            CreatedAt = timeProvider.GetUtcNow().DateTime.ToUniversalTime(),
            Id = Guid.NewGuid().ToString(),
            Password = hash.Aggregate("", (current, b) => current + b.ToString("x2")),
            Role = "User"
        };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var token = CreateJwt(user);
        return new JwtResponse(token);
    }

    private JwtBuilder CreateJwtBuilder()
    {
        return JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA512Algorithm())
            .WithSecret(appOptions.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
            .MustVerifySignature();
    }

    private string CreateJwt(User user)
    {
        return CreateJwtBuilder()
            .AddClaim(nameof(User.Id), user.Id)
            .Encode();
    }
}