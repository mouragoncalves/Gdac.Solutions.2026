using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gdac.Core.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanySegmentAndBlockRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Segment",
                table: "core_companies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SizeCategory",
                table: "core_companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "core_block_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CnpjBase = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BlockedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_block_records", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_core_block_records_CnpjBase",
                table: "core_block_records",
                column: "CnpjBase",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "core_block_records");

            migrationBuilder.DropColumn(
                name: "Segment",
                table: "core_companies");

            migrationBuilder.DropColumn(
                name: "SizeCategory",
                table: "core_companies");
        }
    }
}
