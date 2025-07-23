using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "CartItemsEntities",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "CartItemsEntities");
        }
    }
}
