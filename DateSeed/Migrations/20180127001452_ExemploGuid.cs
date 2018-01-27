using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DateSeed.Migrations
{
    public partial class ExemploGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Descricao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { new Guid("87ab8b47-e8fc-4fd4-8a13-3f03c61886dc"), "Ativo" },
                    { new Guid("3e0f3725-3a7b-4687-a164-58439b9f386a"), "Desativado" },
                    { new Guid("b6ee8a4b-164c-4294-9e46-6368d8c31070"), "SPC" },
                    { new Guid("8972714b-04d9-46e6-b006-40487a6252ed"), "Outros" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Status");
        }
    }
}
