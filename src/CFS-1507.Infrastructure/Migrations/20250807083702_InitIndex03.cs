using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitIndex03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TranslateEntities_translate_name",
                table: "TranslateEntities");

            migrationBuilder.DropIndex(
                name: "IX_ProductEntities_product_name",
                table: "ProductEntities");

            migrationBuilder.CreateIndex(
                name: "IX_TranslateEntities_normalizedName",
                table: "TranslateEntities",
                column: "normalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_ProductEntities_normalizedName",
                table: "ProductEntities",
                column: "normalizedName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TranslateEntities_normalizedName",
                table: "TranslateEntities");

            migrationBuilder.DropIndex(
                name: "IX_ProductEntities_normalizedName",
                table: "ProductEntities");

            migrationBuilder.CreateIndex(
                name: "IX_TranslateEntities_translate_name",
                table: "TranslateEntities",
                column: "translate_name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductEntities_product_name",
                table: "ProductEntities",
                column: "product_name");
        }
    }
}
