using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceitasApi.Migrations
{
    /// <inheritdoc />
    public partial class AllergyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PossiveisAlergias",
                table: "Receitas");

            migrationBuilder.CreateTable(
                name: "Alergias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    ReceitaId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alergias_Receitas_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "Receitas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alergias_ReceitaId",
                table: "Alergias",
                column: "ReceitaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alergias");

            migrationBuilder.AddColumn<string>(
                name: "PossiveisAlergias",
                table: "Receitas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
