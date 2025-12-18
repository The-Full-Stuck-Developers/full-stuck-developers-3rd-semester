using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataccess.Migrations
{
    /// <inheritdoc />
    public partial class Migration18122025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:transaction_type", "deposit,purchase,refund");

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    week_number = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    bet_deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    draw_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    winning_numbers = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    in_person_winners = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    in_person_prize_pool = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("games_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PasswordResetToken = table.Column<string>(type: "text", nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_primary_key", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    mobile_pay_transaction_number = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    type = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("transactions_primary_key", x => x.id);
                    table.ForeignKey(
                        name: "transactions_user_id_users_id_foreign_key",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    selected_numbers = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    is_winning = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Winnings = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    bet_series_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bets_pkey", x => x.id);
                    table.ForeignKey(
                        name: "bets_game_id_fkey",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "bets_transaction_id_fkey",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "bets_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "bets_deleted_at_idx",
                table: "bets",
                column: "deleted_at",
                filter: "\"deleted_at\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_bets_game_id",
                table: "bets",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_bets_transaction_id",
                table: "bets",
                column: "transaction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bets_user_id",
                table: "bets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "games_year_week_key",
                table: "games",
                columns: new[] { "year", "week_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id",
                table: "transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "transactions_mobile_pay_transaction_number_unique",
                table: "transactions",
                column: "mobile_pay_transaction_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_deleted_at_idx",
                table: "users",
                column: "deleted_at",
                filter: "\"deleted_at\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "users_email_unique",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_is_active_idx",
                table: "users",
                column: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bets");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
