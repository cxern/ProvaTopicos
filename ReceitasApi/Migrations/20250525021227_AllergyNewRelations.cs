using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceitasApi.Migrations
{
    /// <inheritdoc />
    public partial class AllergyNewRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alergias_Receitas_ReceitaId",
                table: "Alergias");

            migrationBuilder.AlterColumn<int>(
                name: "ReceitaId",
                table: "Alergias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alergias_Receitas_ReceitaId",
                table: "Alergias",
                column: "ReceitaId",
                principalTable: "Receitas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alergias_Receitas_ReceitaId",
                table: "Alergias");

            migrationBuilder.AlterColumn<int>(
                name: "ReceitaId",
                table: "Alergias",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Alergias_Receitas_ReceitaId",
                table: "Alergias",
                column: "ReceitaId",
                principalTable: "Receitas",
                principalColumn: "Id");
        }
    }
}
