using System.Security.Cryptography;
using dataccess;
using dataccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace api.Security;

public class PasswordHasher: IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(64);
        var hash = GenerateHash(password, salt);
        return $"argon2id${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public PasswordVerificationResult VerifyHashedPassword(
        User user,
        string hashedPassword,
        string providedPassword
    )
    {
        var parts = hashedPassword.Split('$');
        var salt = Convert.FromBase64String(parts[1]);
        var storedHash = Convert.FromBase64String(parts[2]);
        var providedHash = GenerateHash(providedPassword, salt);
        return CryptographicOperations.FixedTimeEquals(storedHash, providedHash)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }

    public byte[] GenerateHash(string password, byte[] salt)
    {
        var hashAlgo = new NSec.Cryptography.Argon2id(new NSec.Cryptography.Argon2Parameters
        {
            DegreeOfParallelism = 1,
            MemorySize = 12288,
            NumberOfPasses = 3,
        });
        return hashAlgo.DeriveBytes(password, salt, 128);
    }
}