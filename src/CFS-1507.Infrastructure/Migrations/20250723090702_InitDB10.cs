using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "product_price",
                table: "CartItemsEntities",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "product_price",
                table: "CartItemsEntities");
        }
    }
}
