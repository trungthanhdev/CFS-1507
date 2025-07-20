using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "translate_description",
                table: "TranslateEntities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "product_description",
                table: "ProductEntities",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "translate_description",
                table: "TranslateEntities");

            migrationBuilder.DropColumn(
                name: "product_description",
                table: "ProductEntities");
        }
    }
}
