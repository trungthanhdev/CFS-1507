using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderEntities_cart_id",
                table: "OrderEntities");

            migrationBuilder.CreateIndex(
                name: "IX_OrderEntities_cart_id",
                table: "OrderEntities",
                column: "cart_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderEntities_cart_id",
                table: "OrderEntities");

            migrationBuilder.CreateIndex(
                name: "IX_OrderEntities_cart_id",
                table: "OrderEntities",
                column: "cart_id",
                unique: true);
        }
    }
}
