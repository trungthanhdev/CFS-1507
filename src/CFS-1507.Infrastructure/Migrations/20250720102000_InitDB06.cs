using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartEntities",
                columns: table => new
                {
                    cart_id = table.Column<string>(type: "text", nullable: false),
                    is_Paid = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartEntities", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_CartEntities_UserEntities_user_id",
                        column: x => x.user_id,
                        principalTable: "UserEntities",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItemsEntities",
                columns: table => new
                {
                    cart_item_id = table.Column<string>(type: "text", nullable: false),
                    cart_id = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemsEntities", x => x.cart_item_id);
                    table.ForeignKey(
                        name: "FK_CartItemsEntities_CartEntities_cart_id",
                        column: x => x.cart_id,
                        principalTable: "CartEntities",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItemsEntities_ProductEntities_product_id",
                        column: x => x.product_id,
                        principalTable: "ProductEntities",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderEntities",
                columns: table => new
                {
                    order_id = table.Column<string>(type: "text", nullable: false),
                    cart_id = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderEntities", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_OrderEntities_CartEntities_cart_id",
                        column: x => x.cart_id,
                        principalTable: "CartEntities",
                        principalColumn: "cart_id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItemsEntities",
                columns: table => new
                {
                    order_item_id = table.Column<string>(type: "text", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemsEntities", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK_OrderItemsEntities_OrderEntities_order_id",
                        column: x => x.order_id,
                        principalTable: "OrderEntities",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartEntities_user_id",
                table: "CartEntities",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemsEntities_cart_id",
                table: "CartItemsEntities",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemsEntities_product_id",
                table: "CartItemsEntities",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderEntities_cart_id",
                table: "OrderEntities",
                column: "cart_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemsEntities_order_id",
                table: "OrderItemsEntities",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItemsEntities");

            migrationBuilder.DropTable(
                name: "OrderItemsEntities");

            migrationBuilder.DropTable(
                name: "OrderEntities");

            migrationBuilder.DropTable(
                name: "CartEntities");
        }
    }
}
