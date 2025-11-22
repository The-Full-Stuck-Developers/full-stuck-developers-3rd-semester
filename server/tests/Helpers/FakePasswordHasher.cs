using Microsoft.AspNetCore.Identity;

namespace tests.Helpers;

class FakePasswordHasher<T> : IPasswordHasher<T>
    where T : class
{
    public string HashPassword(T user, string password) => new string(password.Reverse().ToArray());

    public PasswordVerificationResult VerifyHashedPassword(
        T user,
        string hashedPassword,
        string providedPassword
    ) =>
        Object.Equals(hashedPassword, HashPassword(user, providedPassword))
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
}