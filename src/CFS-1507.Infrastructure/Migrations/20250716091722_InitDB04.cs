using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_BlackListEntities_user_id",
                table: "BlackListEntities",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackListEntities");
        }
    }
}
