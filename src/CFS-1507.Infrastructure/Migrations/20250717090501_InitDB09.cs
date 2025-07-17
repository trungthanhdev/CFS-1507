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
            migrationBuilder.UpdateData(
                table: "UserEntities",
                keyColumn: "user_id",
                keyValue: "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da",
                column: "hashPassWord",
                value: "$2a$11$xkuJ7e8/Je3UxQvwnXvm2u2RymgvdVEu2TO2FqMc8sZpi4RmH4EY6");

            migrationBuilder.UpdateData(
                table: "UserEntities",
                keyColumn: "user_id",
                keyValue: "8dbdf2f7-139b-4037-9f75-4f489313cb12",
                column: "hashPassWord",
                value: "$2a$11$sPYGlkDaP5/0RI8dbJdMzON3Hb/nq8zkUvnWxQHyA/ov.WixVUwDe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserEntities",
                keyColumn: "user_id",
                keyValue: "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da",
                column: "hashPassWord",
                value: "AQAAAAEAACcQAAAAECF8ERMdBibNEje5IGAvzj2lkRLz61RYAGarapHaQ1AmD4XfLVrExybkrp/AD896pQ==");

            migrationBuilder.UpdateData(
                table: "UserEntities",
                keyColumn: "user_id",
                keyValue: "8dbdf2f7-139b-4037-9f75-4f489313cb12",
                column: "hashPassWord",
                value: "AQAAAAEAACcQAAAAEKvJk7QrLvzhRfw4ORSgcB9beNqMImyigYo9EYgtfIywkuBSfPLTncpKVq28Bu66gQ==");
        }
    }
}
