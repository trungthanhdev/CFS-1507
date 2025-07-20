using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranslateEntities",
                columns: table => new
                {
                    translate_id = table.Column<string>(type: "text", nullable: false),
                    translate_name = table.Column<string>(type: "text", nullable: true),
                    translate_price = table.Column<double>(type: "double precision", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    translate_image = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslateEntities", x => x.translate_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslateEntities");
        }
    }
}
