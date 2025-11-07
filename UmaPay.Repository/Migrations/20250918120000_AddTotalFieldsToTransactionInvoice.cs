using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaPay.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalFieldsToTransactionInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "TransactionInvoices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "TransactionInvoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalToPay",
                table: "TransactionInvoices",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "TransactionInvoices");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "TransactionInvoices");

            migrationBuilder.DropColumn(
                name: "TotalToPay",
                table: "TransactionInvoices");
        }
    }
}