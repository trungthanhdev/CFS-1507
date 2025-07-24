using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryEntities",
                columns: table => new
                {
                    category_id = table.Column<string>(type: "text", nullable: false),
                    category_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryEntities", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCateEntities",
                columns: table => new
                {
                    product_cate_id = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<string>(type: "text", nullable: false),
                    category_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCateEntities", x => x.product_cate_id);
                    table.ForeignKey(
                        name: "FK_ProductCateEntities_CategoryEntities_category_id",
                        column: x => x.category_id,
                        principalTable: "CategoryEntities",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCateEntities_ProductEntities_product_id",
                        column: x => x.product_id,
                        principalTable: "ProductEntities",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCateEntities_category_id",
                table: "ProductCateEntities",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCateEntities_product_id",
                table: "ProductCateEntities",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCateEntities");

            migrationBuilder.DropTable(
                name: "CategoryEntities");
        }
    }
}
