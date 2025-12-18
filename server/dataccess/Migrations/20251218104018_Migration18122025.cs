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
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPhysicalPlayers",
                table: "games",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPhysicalPlayers",
                table: "games");
        }
    }
}
