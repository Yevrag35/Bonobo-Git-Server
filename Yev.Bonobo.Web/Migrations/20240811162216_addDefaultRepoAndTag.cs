using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yev.Bonobo.Migrations
{
    /// <inheritdoc />
    public partial class addDefaultRepoAndTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Label = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    AllowAnonymous = table.Column<bool>(type: "INTEGER", nullable: false),
                    AuditPushUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    LinksRegex = table.Column<string>(type: "TEXT", nullable: true),
                    LinksUrl = table.Column<string>(type: "TEXT", nullable: true),
                    LinksUseGlobal = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepoToTag",
                columns: table => new
                {
                    RepoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataTagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepoToTag", x => new { x.RepoId, x.DataTagId });
                    table.ForeignKey(
                        name: "FK_RepoToTag_DataTags_DataTagId",
                        column: x => x.DataTagId,
                        principalTable: "DataTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepoToTag_Repositories_RepoId",
                        column: x => x.RepoId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DataTags",
                columns: new[] { "Id", "Label" },
                values: new object[] { 1, "default" });

            migrationBuilder.InsertData(
                table: "Repositories",
                columns: new[] { "Id", "AllowAnonymous", "AuditPushUser", "Description", "DisplayName", "LinksRegex", "LinksUrl", "LinksUseGlobal", "Name", "Path" },
                values: new object[] { new Guid("02f784d8-56a9-45ba-a380-cc7f08e29b37"), false, true, "The default repository", "Default", null, null, false, "default", "repos" });

            migrationBuilder.CreateIndex(
                name: "IX_RepoToTag_DataTagId",
                table: "RepoToTag",
                column: "DataTagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepoToTag");

            migrationBuilder.DropTable(
                name: "DataTags");

            migrationBuilder.DropTable(
                name: "Repositories");
        }
    }
}
