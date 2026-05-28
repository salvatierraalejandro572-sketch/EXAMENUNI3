using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXAMENUNI3.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Proyectos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_UserId",
                table: "Proyectos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyectos_AspNetUsers_UserId",
                table: "Proyectos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_AspNetUsers_UserId",
                table: "Proyectos");

            migrationBuilder.DropIndex(
                name: "IX_Proyectos_UserId",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Proyectos");
        }
    }
}
