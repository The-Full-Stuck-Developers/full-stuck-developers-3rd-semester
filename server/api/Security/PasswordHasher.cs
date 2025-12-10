using System.Security.Cryptography;
using dataccess;
using dataccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace api.Security;

public class PasswordHasher: IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
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

    public string GenerateRandomPassword(int length = 12)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@$?_-";
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = valid[bytes[i] % valid.Length];
        }

        return new string(chars);
    }
}