using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceitasApi.Migrations
{
    /// <inheritdoc />
    public partial class AddValorToReceita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Valor",
                table: "Receitas",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Receitas");
        }
    }
}
