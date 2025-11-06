using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipService.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenToEquipoMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagen",
                table: "EquiposMedicos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagen",
                table: "EquiposMedicos");
        }
    }
}
