using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaPay.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddSapResponseToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SapResponse",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SapResponse",
                table: "Transactions");
        }
    }
}
