using Microsoft.EntityFrameworkCore.Migrations;

namespace fboAPI.Migrations
{
    public partial class AddingAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerLink",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
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
