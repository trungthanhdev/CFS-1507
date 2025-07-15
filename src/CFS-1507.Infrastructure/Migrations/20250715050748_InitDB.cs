using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguageEntities",
                columns: table => new
                {
                    language_id = table.Column<string>(type: "text", nullable: false),
                    language_name = table.Column<string>(type: "text", nullable: false),
                    language_code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageEntities", x => x.language_id);
                });

            migrationBuilder.InsertData(
                table: "LanguageEntities",
                columns: new[] { "language_id", "language_code", "language_name" },
                values: new object[,]
                {
                    { "0196cce9-cadf-704d-bdcf-9edec1fc115d", "vi-VN", "Tiếng Việt" },
                    { "0196cce9-cadf-7881-894d-b4a5ac0889ff", "en-US", "English" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageEntities");
        }
    }
}
