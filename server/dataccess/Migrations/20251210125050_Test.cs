using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataccess.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_UserId1",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_UserId1",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "transactions");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected")
                .OldAnnotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected,cancelled");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_UserId1",
                table: "transactions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_UserId1",
                table: "transactions",
                column: "UserId1",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
