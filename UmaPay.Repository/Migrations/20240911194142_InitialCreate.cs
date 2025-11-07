using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmaPay.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CurrencyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustormerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Society = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodeSap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    User = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustormerId);
                });

            migrationBuilder.CreateTable(
                name: "Gateways",
                columns: table => new
                {
                    GatewayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gateways", x => x.GatewayId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionStatuses",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatuses", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "GatewayApplications",
                columns: table => new
                {
                    GatewayApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GatewayId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatewayApplications", x => x.GatewayApplicationId);
                    table.ForeignKey(
                        name: "FK_GatewayApplications_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GatewayApplications_Gateways_GatewayId",
                        column: x => x.GatewayId,
                        principalTable: "Gateways",
                        principalColumn: "GatewayId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GatewayCountries",
                columns: table => new
                {
                    GatewayCountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GatewayId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatewayCountries", x => x.GatewayCountryId);
                    table.ForeignKey(
                        name: "FK_GatewayCountries_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GatewayCountries_Gateways_GatewayId",
                        column: x => x.GatewayId,
                        principalTable: "Gateways",
                        principalColumn: "GatewayId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SapDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GatewayRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GatewayPayment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    GatewayApplicationId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustormerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_GatewayApplications_GatewayApplicationId",
                        column: x => x.GatewayApplicationId,
                        principalTable: "GatewayApplications",
                        principalColumn: "GatewayApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "TransactionStatuses",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionInvoices",
                columns: table => new
                {
                    TransactionInvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionInvoices", x => x.TransactionInvoiceId);
                    table.ForeignKey(
                        name: "FK_TransactionInvoices_TransactionStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "TransactionStatuses",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionInvoices_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GatewayApplications_ApplicationId",
                table: "GatewayApplications",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_GatewayApplications_GatewayId",
                table: "GatewayApplications",
                column: "GatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_GatewayCountries_CountryId",
                table: "GatewayCountries",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_GatewayCountries_GatewayId",
                table: "GatewayCountries",
                column: "GatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionInvoices_StatusId",
                table: "TransactionInvoices",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionInvoices_TransactionId",
                table: "TransactionInvoices",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CountryId",
                table: "Transactions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CustomerId",
                table: "Transactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GatewayApplicationId",
                table: "Transactions",
                column: "GatewayApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_StatusId",
                table: "Transactions",
                column: "StatusId");

            Seed(migrationBuilder);
        }

        private void Seed(MigrationBuilder migrationBuilder)
        {
            // Seed Countries
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "Name", "CurrencyCode", "CurrencyName" },
                values: new object[,]
                {
                    { 1, "COL", "COP", "Peso Colombiano" }
                }
            );

            // Seed Gateways
            migrationBuilder.InsertData(
                table: "Gateways",
                columns: new[] { "GatewayId", "Name", "Code" },
                values: new object[,]
                {
                    { 1, "Davivienda", "DAVI_COL" }
                }
            );

            // Seed Countries
            migrationBuilder.InsertData(
                table: "GatewayCountries",
                columns: new[] { "GatewayId", "CountryId" },
                values: new object[,]
                {
                    { 1, 1 }
                }
                );

            migrationBuilder.InsertData(
                  table: "Applications",
                  columns: new[] { "ApplicationId", "Name", "ApiKey", "Secret", "IsActive", "CreatedAt", "LastUpdated" },
                  values: new object[,]
                  {
                      { 1, "UmaOne", "IpZxq42vjP7nuI0RZCgJ/gvlFskSI0lN","Kgekmvu+EDg1jex2sx6fh8uArhL01F2z9Dx8AF6MEIcJPJOF/q6bRrhui3mztxX2", true, DateTime.UtcNow, DateTime.UtcNow}
                  }
              );


            migrationBuilder.InsertData(
                table: "GatewayApplications",
                columns: new[] { "GatewayId", "ApplicationId" },
                values: new object[,]
                {
                    { 1, 1}
                }
            );


            // Seed Gateways
            migrationBuilder.InsertData(
                table: "TransactionStatuses",
                columns: new[] { "StatusId", "Name", "Description" },
                values: new object[,]
                {
                     { 1, "Initiated", "Iniciada" },
                     { 2, "Processing", "Procesando" },
                     { 3, "Completed", "Completada" },
                     { 4, "Failed", "Fallida" },
                     { 5, "Cancelled", "Cancelada" },
                     { 6, "CompletedInSap", "Completada en SAP" },
                     { 7, "FailedInSap", "allida en " },
                     { 8, "GatewaySucess", "Consulta URL Sucess" },
                     { 9, "GatewayFailure", "Consulta URL Failure" },
                     { 10, "GatewayPending", "Consulta URL Pending" },
                     { 11, "GatewayReview", "Consulta URL Review" },

                }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GatewayCountries");

            migrationBuilder.DropTable(
                name: "TransactionInvoices");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "GatewayApplications");

            migrationBuilder.DropTable(
                name: "TransactionStatuses");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Gateways");
        }
    }
}
