using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fboAPI.Migrations
{
    public partial class InitialLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerLink",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    OldID = table.Column<int>(nullable: false),
                    NewID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLink", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerLink");
        }
    }
}
