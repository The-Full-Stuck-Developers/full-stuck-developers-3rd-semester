using System.Security.Claims;
using System.Text;
using api.Models.Dtos.Responses;
using api.Security;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace api.Services;

public interface ITokenService
{
    string CreateToken(AuthUserInfo user);
}


public class JwtService(IConfiguration config) : ITokenService
{
    public const string SignatureAlgorithm = SecurityAlgorithms.HmacSha512;
    public const string JwtSecret = "JwtSecret";

    public string CreateToken(AuthUserInfo user)
    {
        var key = Convert.FromBase64String(config.GetValue<string>(JwtSecret)!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SignatureAlgorithm
            ),
            Subject = new ClaimsIdentity(user.ToClaims()),
            Expires = DateTime.UtcNow.AddDays(7),
        };
        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }

    public static TokenValidationParameters CreateValidationParams(IConfiguration configuration)
    {
        var secret = configuration["AppOptions:JwtSecret"]
                     ?? throw new Exception("Jwt secret not found!");

        //var key = Convert.FromBase64String(secret);
        var key = Encoding.UTF8.GetBytes(secret);
        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidAlgorithms = [SignatureAlgorithm],
            ValidateIssuerSigningKey = true,
            TokenDecryptionKey = null,

            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,

            // Set to 0 when validating on the same system that created the token
            ClockSkew = TimeSpan.Zero,
            
            /*
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt: Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppOptions:JwtSecret"] ?? throw new InvalidOperationException("JWT Key is missing in configuration")))
        */
        }; 
    }
}