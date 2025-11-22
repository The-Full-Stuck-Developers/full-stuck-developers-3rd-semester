using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataccess.Migrations
{
    /// <inheritdoc />
    public partial class MakeDeletedAtNullableInUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "users",
                type: "timestamp with time zone", 
                nullable: true,                   
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: false);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
