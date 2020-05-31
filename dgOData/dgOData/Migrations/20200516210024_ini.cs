using Microsoft.EntityFrameworkCore.Migrations;

namespace dgOData.Migrations
{
    public partial class ini : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escolas",
                columns: table => new
                {
                    EscolaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(maxLength: 80, nullable: false),
                    Estado = table.Column<string>(maxLength: 2, nullable: false),
                    Cidade = table.Column<string>(maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escolas", x => x.EscolaId);
                });

            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    AlunoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(maxLength: 80, nullable: false),
                    Nota = table.Column<int>(nullable: false),
                    EscolaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.AlunoId);
                    table.ForeignKey(
                        name: "FK_Alunos_Escolas_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escolas",
                        principalColumn: "EscolaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_EscolaId",
                table: "Alunos",
                column: "EscolaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alunos");

            migrationBuilder.DropTable(
                name: "Escolas");
        }
    }
}
