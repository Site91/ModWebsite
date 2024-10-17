using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mod.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class siteroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SiteRoles",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SiteRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SafeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    canRemove = table.Column<bool>(type: "bit", nullable: false),
                    canModify = table.Column<bool>(type: "bit", nullable: false),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteRoles_AuthorizedSites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "AuthorizedSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSiteRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSiteRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSiteRole_SiteRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SiteRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteRoles_SiteId",
                table: "SiteRoles",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSiteRole_RoleId",
                table: "UserSiteRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSiteRole");

            migrationBuilder.DropTable(
                name: "SiteRoles");

            migrationBuilder.DropColumn(
                name: "SiteRoles",
                table: "AspNetUsers");
        }
    }
}
