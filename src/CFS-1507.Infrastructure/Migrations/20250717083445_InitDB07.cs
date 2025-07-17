using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFS_1507.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB07 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attachToEntities_RoleEntities_role_id",
                table: "attachToEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_attachToEntities_UserEntities_user_id",
                table: "attachToEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_attachToEntities",
                table: "attachToEntities");

            migrationBuilder.RenameTable(
                name: "attachToEntities",
                newName: "AttachToEntities");

            migrationBuilder.RenameIndex(
                name: "IX_attachToEntities_user_id",
                table: "AttachToEntities",
                newName: "IX_AttachToEntities_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_attachToEntities_role_id",
                table: "AttachToEntities",
                newName: "IX_AttachToEntities_role_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttachToEntities",
                table: "AttachToEntities",
                column: "attach_to_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachToEntities_RoleEntities_role_id",
                table: "AttachToEntities",
                column: "role_id",
                principalTable: "RoleEntities",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttachToEntities_UserEntities_user_id",
                table: "AttachToEntities",
                column: "user_id",
                principalTable: "UserEntities",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachToEntities_RoleEntities_role_id",
                table: "AttachToEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_AttachToEntities_UserEntities_user_id",
                table: "AttachToEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttachToEntities",
                table: "AttachToEntities");

            migrationBuilder.RenameTable(
                name: "AttachToEntities",
                newName: "attachToEntities");

            migrationBuilder.RenameIndex(
                name: "IX_AttachToEntities_user_id",
                table: "attachToEntities",
                newName: "IX_attachToEntities_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AttachToEntities_role_id",
                table: "attachToEntities",
                newName: "IX_attachToEntities_role_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_attachToEntities",
                table: "attachToEntities",
                column: "attach_to_id");

            migrationBuilder.AddForeignKey(
                name: "FK_attachToEntities_RoleEntities_role_id",
                table: "attachToEntities",
                column: "role_id",
                principalTable: "RoleEntities",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_attachToEntities_UserEntities_user_id",
                table: "attachToEntities",
                column: "user_id",
                principalTable: "UserEntities",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
