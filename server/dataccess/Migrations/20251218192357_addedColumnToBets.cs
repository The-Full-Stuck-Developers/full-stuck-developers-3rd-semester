using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataccess.Migrations
{
    /// <inheritdoc />
    public partial class addedColumnToBets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "bet_series_id",
                table: "bets",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bet_series_id",
                table: "bets");
        }
    }
}
