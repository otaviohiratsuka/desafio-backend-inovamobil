using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaBancaria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelaIdempotencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChavesIdempotencia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChavesIdempotencia", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChavesIdempotencia");
        }
    }
}
