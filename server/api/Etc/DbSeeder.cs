using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task Seed(string defaultPassword)
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

        if (!await context.Games.AnyAsync())
        {
            // Start from current year (or last year for past game history)
            var startYear = DateTime.UtcNow.Year;
            await CreateGames(startYear, 50);
        }

        if (!await context.Transactions.AnyAsync())
        {
            await CreateTransactions(100);
        }
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

    private async Task CreateGames(int startYear, int numberOfYears)
    {
        var random = new Random();

        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentWeek = ISOWeek.GetWeekOfYear(now);

        int pastYear = startYear - 1;
        int pastWeeks = ISOWeek.GetWeeksInYear(pastYear);

        for (int week = 1; week <= pastWeeks; week++)
        {
            DateTime weekMonday = DateTime.SpecifyKind(
                ISOWeek.ToDateTime(pastYear, week, DayOfWeek.Monday),
                DateTimeKind.Utc
            );

            DateTime startTime = new DateTime(
                weekMonday.Year,
                weekMonday.Month,
                weekMonday.Day,
                0, 1, 0,
                DateTimeKind.Utc
            );

            DateTime betDeadline = new DateTime(
                weekMonday.AddDays(5).Year,
                weekMonday.AddDays(5).Month,
                weekMonday.AddDays(5).Day,
                17, 0, 0,
                DateTimeKind.Utc
            );

            int drawMinute = random.Next(0, 60);

            DateTime drawDate = new DateTime(
                weekMonday.AddDays(6).Year,
                weekMonday.AddDays(6).Month,
                weekMonday.AddDays(6).Day,
                17, drawMinute, 0,
                DateTimeKind.Utc
            );

            var isPastGame =
                pastYear < currentYear ||
                (pastYear == currentYear && week < currentWeek);

            string? winningNumbers = null;
            int revenue = 0;

            if (isPastGame)
            {
                winningNumbers = string.Join(",",
                    Enumerable.Range(1, 16)
                        .OrderBy(_ => random.Next())
                        .Take(3)
                        .OrderBy(n => n)
                        .Select(n => n.ToString())
                );

                revenue = random.Next(50_000, 500_000);
            }

            var game = new Game
            {
                Id = Guid.NewGuid(),
                WeekNumber = week,
                Year = pastYear,
                StartTime = startTime,
                BetDeadline = betDeadline,
                DrawDate = drawDate,
                WinningNumbers = winningNumbers
            };

            context.Games.Add(game);
        }

        for (int year = startYear; year < startYear + numberOfYears; year++)
        {
            // Use ISO 8601 week numbering (used in Denmark)
            int weeksInYear = ISOWeek.GetWeeksInYear(year);

            for (int week = 1; week <= weeksInYear; week++)
            {
                // Get Monday of this ISO week
                DateTime weekMonday = DateTime.SpecifyKind(
                    ISOWeek.ToDateTime(year, week, DayOfWeek.Monday),
                    DateTimeKind.Utc
                );
                // Game starts Monday at 00:01
                DateTime startTime = new DateTime(
                    weekMonday.Year,
                    weekMonday.Month,
                    weekMonday.Day,
                    0, 1, 0,
                    DateTimeKind.Utc
                );

                // Bet deadline: Saturday at 17:00 (5 PM)
                DateTime saturdayDeadline = weekMonday.AddDays(5);
                DateTime betDeadline = new DateTime(
                    saturdayDeadline.Year,
                    saturdayDeadline.Month,
                    saturdayDeadline.Day,
                    17, 0, 0,
                    DateTimeKind.Utc
                );

                var game = new Game
                {
                    Id = Guid.NewGuid(),
                    WeekNumber = week,
                    Year = year,
                    StartTime = startTime,
                    BetDeadline = betDeadline,
                    DrawDate = null,
                    WinningNumbers = null,
                };

                context.Games.Add(game);
            }
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

        for (int i = 0; i < 30; i++)
        {
            var transaction = new Transaction
            {
                UserId = users[random.Next(users.Count)].Id,
                Amount = random.Next(500, 1500),
                MobilePayTransactionNumber = random.Next(100000, 999999),
                Status = TransactionStatus.Accepted,
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
