using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "CartItemsEntities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "CartItemsEntities",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "CartItemsEntities");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "CartItemsEntities");
        }
    }
}
