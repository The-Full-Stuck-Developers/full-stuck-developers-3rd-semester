using api.Etc;
using dataccess;
using dataccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Etc;

public class DbSeeder(
    MyDbContext context,
    IPasswordHasher<User> hasher
)

{
    public async Task Seed(String defaultPassword)
    {
        // 1. Ensure Database Exists
        context.Database.EnsureCreated();
        if (!await context.Users.AnyAsync())
        {
            await CreateUsers(
                [
                    (email: "admin@example.com", name: "Jesus", phoneNumber: "55225522", isAdmin: true),
                    (email: "user@example.com", "Fred", "53998832", isAdmin: false),
                    (email: "player@example.com", "Peter", "912043984", isAdmin: false),
                ],
                defaultPassword
            );
        }

        if (!await context.Transactions.AnyAsync())
        {
            await CreateTransactions(100);
        }

        // if (!await context.Games.AnyAsync())
        // {
        //     context.Games.Add(
        //         new Game
        //         {
        //             StartTime = DateTime.UtcNow,
        //             // IsActive = true,
        //             Revenue = 0,
        //             WinningNumbers = null
        //         });
        //     await context.SaveChangesAsync();
        // }
    }

    private async Task CreateUsers((string email, string name, string phoneNumber, bool isAdmin)[] users,
        string defaultPassword)
    {
        foreach (var userData in users)
        {
            var user = new User
            {
                Email = userData.email,
                Name = userData.name,
                PhoneNumber = userData.phoneNumber,
                IsAdmin = userData.isAdmin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PasswordHash = hasher.HashPassword(null, defaultPassword),
            };

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }

    private async Task CreateTransactions(int count)
    {
        var users = await context.Users.ToListAsync();
        var random = new Random();
        var statuses = Enum.GetValues<TransactionStatus>();
        var types = Enum.GetValues<TransactionType>();
        int[] boardPrices = { 20, 40, 80, 160 };

        for (int i = 0; i < count; i++)
        {
            var transaction = new Transaction
            {
                UserId = users[random.Next(users.Count)].Id,
                Amount = random.Next(500, 1500),
                MobilePayTransactionNumber = random.Next(100000, 999999),
                Status = statuses[random.Next(statuses.Length)],
                Type = TransactionType.Deposit,
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30))
            };

            context.Transactions.Add(transaction);
        }

        for (int i = 0; i < count; i++)
        {
            var transaction = new Transaction
            {
                UserId = users[random.Next(users.Count)].Id,
                Amount = boardPrices[random.Next(boardPrices.Length)],
                Status = TransactionStatus.Accepted,
                Type = TransactionType.Purchase,
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30))
            };

            context.Transactions.Add(transaction);
        }

        await context.SaveChangesAsync();
    }
}
