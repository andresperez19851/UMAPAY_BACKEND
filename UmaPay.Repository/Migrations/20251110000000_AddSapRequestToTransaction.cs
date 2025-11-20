using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaPay.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddSapRequestToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SapRequest",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SapRequest",
                table: "Transactions");
        }
    }
}
