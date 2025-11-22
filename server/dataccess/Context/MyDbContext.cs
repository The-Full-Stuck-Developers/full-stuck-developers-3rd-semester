using dataccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace dataccess;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);


        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_primary_key");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password")
                .HasMaxLength(512)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(256)
                .IsRequired();

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("users_email_unique");

            entity.Property(e => e.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.IsAdmin)
                .HasColumnName("is_admin")
                .HasDefaultValue(false);

            entity.Property(e => e.ExpiresAt)
                .HasColumnName("expires_at");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("games_primary_key");

            entity.ToTable("games");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.StartTime)
                .HasColumnName("start_time");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(false);

            entity.Property(e => e.WinningNumbers)
                .HasColumnName("winning_numbers")
                .HasMaxLength(64);

            entity.Property(e => e.Revenue)
                .HasColumnName("revenue")
                .IsRequired();
        });

        modelBuilder.HasPostgresEnum<TransactionStatus>();

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_primary_key");

            entity.ToTable("transactions");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .IsRequired();

            entity.Property(e => e.MobilePayTransactionNumber)
                .HasColumnName("mobile_pay_transaction_number")
                .IsRequired();

            entity.HasIndex(e => e.MobilePayTransactionNumber)
                .IsUnique()
                .HasDatabaseName("transactions_mobile_pay_transaction_number_unique");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(TransactionStatus.Pending)
                .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("transactions_user_id_users_id_foreign_key");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}