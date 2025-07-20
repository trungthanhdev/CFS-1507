using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "product_id",
                table: "TranslateEntities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TranslateEntities_product_id",
                table: "TranslateEntities",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_TranslateEntities_ProductEntities_product_id",
                table: "TranslateEntities",
                column: "product_id",
                principalTable: "ProductEntities",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranslateEntities_ProductEntities_product_id",
                table: "TranslateEntities");

            migrationBuilder.DropIndex(
                name: "IX_TranslateEntities_product_id",
                table: "TranslateEntities");

            migrationBuilder.DropColumn(
                name: "product_id",
                table: "TranslateEntities");
        }
    }
}
