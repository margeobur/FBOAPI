using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fboAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    StringsAsString = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OldCustomer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLink",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    OldID = table.Column<int>(nullable: false),
                    NewID = table.Column<string>(nullable: true),
                    OldDataId = table.Column<int>(nullable: true),
                    NewDataId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLink", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerLink_NewCustomer_NewDataId",
                        column: x => x.NewDataId,
                        principalTable: "NewCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerLink_OldCustomer_OldDataId",
                        column: x => x.OldDataId,
                        principalTable: "OldCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLink_NewDataId",
                table: "CustomerLink",
                column: "NewDataId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLink_OldDataId",
                table: "CustomerLink",
                column: "OldDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerLink");

            migrationBuilder.DropTable(
                name: "NewCustomer");

            migrationBuilder.DropTable(
                name: "OldCustomer");
        }
    }
}
