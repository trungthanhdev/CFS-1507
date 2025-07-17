using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "user_id", "email", "hashPassWord", "userName" },
                values: new object[,]
                {
                    { "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da", "admin@example.com", "AQAAAAEAACcQAAAAECF8ERMdBibNEje5IGAvzj2lkRLz61RYAGarapHaQ1AmD4XfLVrExybkrp/AD896pQ==", "root" },
                    { "8dbdf2f7-139b-4037-9f75-4f489313cb12", "user1@example.com", "AQAAAAEAACcQAAAAEKvJk7QrLvzhRfw4ORSgcB9beNqMImyigYo9EYgtfIywkuBSfPLTncpKVq28Bu66gQ==", "dev" }
                });

            migrationBuilder.InsertData(
                table: "AttachToEntities",
                columns: new[] { "attach_to_id", "role_id", "user_id" },
                values: new object[,]
                {
                    { "09fc9342-3bc3-4a01-81d9-2c38e6b6f5c4", "f8e7280b-37c3-41d1-9a2d-6a1f40b25cd3", "8dbdf2f7-139b-4037-9f75-4f489313cb12" },
                    { "6e4f964b-8c79-4c7f-8db7-5c9df6b3a131", "a47a25b5-6ef4-47b4-b942-52c2525a9a56", "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AttachToEntities",
                keyColumn: "attach_to_id",
                keyValue: "09fc9342-3bc3-4a01-81d9-2c38e6b6f5c4");

            migrationBuilder.DeleteData(
                table: "AttachToEntities",
                keyColumn: "attach_to_id",
                keyValue: "6e4f964b-8c79-4c7f-8db7-5c9df6b3a131");

            migrationBuilder.DeleteData(
                table: "RoleEntities",
                keyColumn: "role_id",
                keyValue: "a47a25b5-6ef4-47b4-b942-52c2525a9a56");

            migrationBuilder.DeleteData(
                table: "RoleEntities",
                keyColumn: "role_id",
                keyValue: "f8e7280b-37c3-41d1-9a2d-6a1f40b25cd3");

            migrationBuilder.DeleteData(
                table: "UserEntities",
                keyColumn: "user_id",
                keyValue: "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da");

            migrationBuilder.DeleteData(
                table: "UserEntities",
                keyColumn: "user_id",
                keyValue: "8dbdf2f7-139b-4037-9f75-4f489313cb12");
        }
    }
}
