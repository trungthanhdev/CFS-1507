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
                name: "attachToEntities",
                columns: table => new
                {
                    attach_to_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachToEntities", x => x.attach_to_id);
                    table.ForeignKey(
                        name: "FK_attachToEntities_RoleEntities_role_id",
                        column: x => x.role_id,
                        principalTable: "RoleEntities",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attachToEntities_UserEntities_user_id",
                        column: x => x.user_id,
                        principalTable: "UserEntities",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attachToEntities_role_id",
                table: "attachToEntities",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_attachToEntities_user_id",
                table: "attachToEntities",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachToEntities");

            migrationBuilder.DropTable(
                name: "RoleEntities");
        }
    }
}
