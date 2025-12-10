using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dataccess;
using dataccess.Seeders;

namespace dataccess.Seeders;

public class TransactionSeeder : ISeeder
{
    public async Task SeedAsync(MyDbContext db)
    {
        if (!db.Users.Any()) return;
        if (db.Transactions.Any()) return;

        var firstUser = db.Users.First();

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = firstUser.Id,
                Amount = 500,
                MobilePayTransactionNumber = 111111,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = firstUser.Id,
                Amount = 200,
                MobilePayTransactionNumber = 222222,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            }
        };

        await db.Transactions.AddRangeAsync(transactions);
        await db.SaveChangesAsync();
    }
}