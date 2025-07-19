using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguageEntities",
                columns: table => new
                {
                    language_id = table.Column<string>(type: "text", nullable: false),
                    language_name = table.Column<string>(type: "text", nullable: false),
                    language_code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageEntities", x => x.language_id);
                });

            migrationBuilder.CreateTable(
                name: "ProductEntities",
                columns: table => new
                {
                    product_id = table.Column<string>(type: "text", nullable: false),
                    product_name = table.Column<string>(type: "text", nullable: true),
                    product_price = table.Column<double>(type: "double precision", nullable: true),
                    product_image = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductEntities", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "RoleEntities",
                columns: table => new
                {
                    role_id = table.Column<string>(type: "text", nullable: false),
                    role_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleEntities", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "UserEntities",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    userName = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    hashPassWord = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntities", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "AttachToEntities",
                columns: table => new
                {
                    attach_to_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachToEntities", x => x.attach_to_id);
                    table.ForeignKey(
                        name: "FK_AttachToEntities_RoleEntities_role_id",
                        column: x => x.role_id,
                        principalTable: "RoleEntities",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttachToEntities_UserEntities_user_id",
                        column: x => x.user_id,
                        principalTable: "UserEntities",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlackListEntities",
                columns: table => new
                {
                    blacklist_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    create_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    token_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackListEntities", x => x.blacklist_id);
                    table.ForeignKey(
                        name: "FK_BlackListEntities_UserEntities_user_id",
                        column: x => x.user_id,
                        principalTable: "UserEntities",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LanguageEntities",
                columns: new[] { "language_id", "language_code", "language_name" },
                values: new object[,]
                {
                    { "0196cce9-cadf-704d-bdcf-9edec1fc115d", "vi-VN", "Tiếng Việt" },
                    { "0196cce9-cadf-7881-894d-b4a5ac0889ff", "en-US", "English" }
                });

            migrationBuilder.InsertData(
                table: "RoleEntities",
                columns: new[] { "role_id", "role_name" },
                values: new object[,]
                {
                    { "a47a25b5-6ef4-47b4-b942-52c2525a9a56", "ADMIN" },
                    { "f8e7280b-37c3-41d1-9a2d-6a1f40b25cd3", "USER" }
                });

            migrationBuilder.InsertData(
                table: "UserEntities",
                columns: new[] { "user_id", "created_at", "email", "hashPassWord", "updated_at", "userName" },
                values: new object[,]
                {
                    { "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin@example.com", "$2a$11$KNZLsWhag2eHt2FvvO/Zp.BfDDarMVYA8xMRlJmCt9iHREew38wme", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "root" },
                    { "8dbdf2f7-139b-4037-9f75-4f489313cb12", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "user1@example.com", "$2a$11$KNZLsWhag2eHt2FvvO/Zp.BfDDarMVYA8xMRlJmCt9iHREew38wme", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "dev" }
                });

            migrationBuilder.InsertData(
                table: "AttachToEntities",
                columns: new[] { "attach_to_id", "created_at", "role_id", "updated_at", "user_id" },
                values: new object[,]
                {
                    { "09fc9342-3bc3-4a01-81d9-2c38e6b6f5c4", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "f8e7280b-37c3-41d1-9a2d-6a1f40b25cd3", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "8dbdf2f7-139b-4037-9f75-4f489313cb12" },
                    { "6e4f964b-8c79-4c7f-8db7-5c9df6b3a131", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "a47a25b5-6ef4-47b4-b942-52c2525a9a56", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachToEntities_role_id",
                table: "AttachToEntities",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_AttachToEntities_user_id",
                table: "AttachToEntities",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_BlackListEntities_user_id",
                table: "BlackListEntities",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachToEntities");

            migrationBuilder.DropTable(
                name: "BlackListEntities");

            migrationBuilder.DropTable(
                name: "LanguageEntities");

            migrationBuilder.DropTable(
                name: "ProductEntities");

            migrationBuilder.DropTable(
                name: "RoleEntities");

            migrationBuilder.DropTable(
                name: "UserEntities");
        }
    }
}
