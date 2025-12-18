using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataccess.Migrations
{
    /// <inheritdoc />
    public partial class Migration181220252 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InPersonWinners",
                table: "games",
                newName: "number_of_physical_players");

            migrationBuilder.AlterColumn<int>(
                name: "number_of_physical_players",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "number_of_physical_players",
                table: "games",
                newName: "InPersonWinners");

            migrationBuilder.AlterColumn<int>(
                name: "InPersonWinners",
                table: "games",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);
        }
    }
}
