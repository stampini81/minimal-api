using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mininal_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedVeiculos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Veiculos",
                columns: new[] { "Id", "Nome", "Marca", "Ano" },
                values: new object[,]
                {
                    { 1, "Fusca", "Volkswagen", 1980 },
                    { 2, "Gol", "Volkswagen", 2005 },
                    { 3, "Uno", "Fiat", 1998 }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Veiculos", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Veiculos", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Veiculos", keyColumn: "Id", keyValue: 3);
        }
    }
}
