using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaPay.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddSapDocumentNumberToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionStatusLogs_TransactionStatuses_StatusId",
                table: "TransactionStatusLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionStatusLogs_Transactions_TransactionId",
                table: "TransactionStatusLogs");

            migrationBuilder.AddColumn<string>(
                name: "SapDocument",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionStatusLogs_TransactionStatuses_StatusId",
                table: "TransactionStatusLogs",
                column: "StatusId",
                principalTable: "TransactionStatuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionStatusLogs_Transactions_TransactionId",
                table: "TransactionStatusLogs",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionStatusLogs_TransactionStatuses_StatusId",
                table: "TransactionStatusLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionStatusLogs_Transactions_TransactionId",
                table: "TransactionStatusLogs");

            migrationBuilder.DropColumn(
                name: "SapDocument",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionStatusLogs_TransactionStatuses_StatusId",
                table: "TransactionStatusLogs",
                column: "StatusId",
                principalTable: "TransactionStatuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionStatusLogs_Transactions_TransactionId",
                table: "TransactionStatusLogs",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
