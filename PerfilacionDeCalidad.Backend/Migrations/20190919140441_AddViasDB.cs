using Microsoft.EntityFrameworkCore.Migrations;

namespace PerfilacionDeCalidad.Backend.Migrations
{
    public partial class AddViasDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TypeVias",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    TypeVia = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeVias", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeVias");
        }
    }
}
