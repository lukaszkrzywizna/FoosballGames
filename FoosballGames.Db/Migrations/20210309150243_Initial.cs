using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FoosballGames.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "FoosballGames");

            migrationBuilder.CreateTable(
                name: "FoosballGames",
                schema: "FoosballGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JsonContent = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_FoosballGames", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoosballGames",
                schema: "FoosballGames");
        }
    }
}