using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PerfilacionDeCalidad.Backend.Migrations
{
    public partial class AddTiposDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeDocument",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "TypeDocumentID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TypeDocuments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Abreviatura = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeDocuments", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TypeDocumentID",
                table: "AspNetUsers",
                column: "TypeDocumentID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TypeDocuments_TypeDocumentID",
                table: "AspNetUsers",
                column: "TypeDocumentID",
                principalTable: "TypeDocuments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TypeDocuments_TypeDocumentID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TypeDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TypeDocumentID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TypeDocumentID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "TypeDocument",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
        }
    }
}
