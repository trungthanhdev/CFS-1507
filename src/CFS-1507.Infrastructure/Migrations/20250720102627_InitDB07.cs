using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB07 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "in_stock",
                table: "ProductEntities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "is_bought",
                table: "ProductEntities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "is_in_cart",
                table: "ProductEntities",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "in_stock",
                table: "ProductEntities");

            migrationBuilder.DropColumn(
                name: "is_bought",
                table: "ProductEntities");

            migrationBuilder.DropColumn(
                name: "is_in_cart",
                table: "ProductEntities");
        }
    }
}
