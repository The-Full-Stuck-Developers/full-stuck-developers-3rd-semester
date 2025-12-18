using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundTransactionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:transaction_type", "deposit,purchase,refund")
                .OldAnnotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:transaction_type", "deposit,purchase");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:transaction_type", "deposit,purchase")
                .OldAnnotation("Npgsql:Enum:transaction_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:transaction_type", "deposit,purchase,refund");
        }
    }
}
