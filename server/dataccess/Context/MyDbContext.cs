using dataccess.Entities;
using DefaultNamespace;
using domain;
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
    public virtual DbSet<Bet> Bets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);


        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_primary_key");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

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

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(false);

            entity.Property(e => e.ExpiresAt)
                .HasColumnName("expires_at");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at");

            entity.HasIndex(e => e.DeletedAt)
                .HasFilter("\"deleted_at\" IS NULL")
                .HasDatabaseName("users_deleted_at_idx");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("users_is_active_idx");

            entity.HasMany(u => u.Bets)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .HasConstraintName("bets_user_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .HasConstraintName("transactions_user_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("games_pkey");

            entity.ToTable("games");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.WeekNumber)
                .HasColumnName("week_number")
                .IsRequired();

            entity.Property(e => e.Year)
                .HasColumnName("year")
                .IsRequired();

            entity.HasIndex(e => new { e.Year, e.WeekNumber })
                .IsUnique()
                .HasDatabaseName("games_year_week_key");

            entity.Property(e => e.StartTime)
                .HasColumnName("start_time")
                .IsRequired();

            entity.Property(e => e.BetDeadline)
                .HasColumnName("bet_deadline")
                .IsRequired();

            entity.Property(e => e.DrawDate)
                .HasColumnName("draw_date");


            entity.Property(e => e.WinningNumbers)
                .HasColumnName("winning_numbers")
                .HasMaxLength(64);

            entity.Property(e=> e.NumberOfPhysicalPlayers)
                .HasColumnName("number_of_physical_players")
                .HasDefaultValue(0)
                .IsRequired();

            entity.HasMany(g => g.Bets)
                .WithOne(b => b.Game)
                .HasForeignKey(b => b.GameId)
                .HasConstraintName("bets_game_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);
    entity.Property(e => e.WinningNumbers)
        .HasColumnName("winning_numbers")
        .HasMaxLength(64); 
    
    entity.HasMany(g => g.Bets)
        .WithOne(b => b.Game)
        .HasForeignKey(b => b.GameId)
        .HasConstraintName("Bet_game_id_fkey")
        .OnDelete(DeleteBehavior.Cascade);
    
        });

        modelBuilder.HasPostgresEnum<TransactionStatus>();
        modelBuilder.HasPostgresEnum<TransactionType>();

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_primary_key");

            entity.ToTable("transactions");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .IsRequired();

            entity.Property(e => e.MobilePayTransactionNumber)
                .HasColumnName("mobile_pay_transaction_number")
                .IsRequired(false)
                .ValueGeneratedNever();

            entity.HasIndex(e => e.MobilePayTransactionNumber)
                .IsUnique()
                .HasDatabaseName("transactions_mobile_pay_transaction_number_unique");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(TransactionStatus.Pending)
                .HasConversion<string>();

            entity.Property(e => e.Type)
                .HasColumnName("type")
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("transactions_user_id_users_id_foreign_key")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Bet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bets_pkey");

            entity.ToTable("bets");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.GameId)
                .HasColumnName("game_id")
                .IsRequired();

            entity.Property(e => e.SelectedNumbers)
                .HasColumnName("selected_numbers")
                .HasMaxLength(24)
                .IsRequired();

            entity.Property(e => e.TransactionId)
                .HasColumnName("transaction_id")
                .IsRequired();

            entity.Property(e => e.IsWinning)
                .HasColumnName("is_winning")
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at");

            entity.HasIndex(e => e.DeletedAt)
                .HasFilter("\"deleted_at\" IS NULL")
                .HasDatabaseName("bets_deleted_at_idx");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("bets_user_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(e => e.GameId)
                .HasConstraintName("bets_game_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Transaction)
                .WithOne(t => t.Bet)
                .HasForeignKey<Bet>(b => b.TransactionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bets_transaction_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
