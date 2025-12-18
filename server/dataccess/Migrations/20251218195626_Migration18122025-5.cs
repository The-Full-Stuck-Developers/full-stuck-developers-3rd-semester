using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataccess.Migrations
{
    /// <inheritdoc />
    public partial class Migration181220255 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "number_of_physical_players",
                table: "games",
                newName: "in_person_winners");

            migrationBuilder.AddColumn<int>(
                name: "in_person_prize_pool",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "in_person_prize_pool",
                table: "games");

            migrationBuilder.RenameColumn(
                name: "in_person_winners",
                table: "games",
                newName: "number_of_physical_players");
        }
    }
}
