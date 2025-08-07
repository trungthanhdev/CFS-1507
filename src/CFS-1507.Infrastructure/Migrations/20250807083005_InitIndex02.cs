using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitIndex02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "normalizedName",
                table: "TranslateEntities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalizedName",
                table: "ProductEntities",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "normalizedName",
                table: "TranslateEntities");

            migrationBuilder.DropColumn(
                name: "normalizedName",
                table: "ProductEntities");
        }
    }
}
