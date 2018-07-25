using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace StartUpApi.Migrations
{
    public partial class userPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationRolePermission_Permissions_permissionId",
                table: "ApplicationRolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Permissions_parentid",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "permissionID",
                table: "Permissions");

            migrationBuilder.RenameColumn(
                name: "parentid",
                table: "Permissions",
                newName: "ParentId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Permissions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "level",
                table: "Permissions",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "displayname",
                table: "Permissions",
                newName: "DisplayName");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_parentid",
                table: "Permissions",
                newName: "IX_Permissions_ParentId");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "Permissions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "ApplicationUserRole",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "UserPermission",
                columns: table => new
                {
                    permissionId = table.Column<int>(nullable: false),
                    userId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermission", x => new { x.permissionId, x.userId });
                    table.ForeignKey(
                        name: "FK_UserPermission_Permissions_permissionId",
                        column: x => x.permissionId,
                        principalTable: "Permissions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermission_ApplicationUser_userId",
                        column: x => x.userId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_userId",
                table: "UserPermission",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRolePermission_Permissions_permissionId",
                table: "ApplicationRolePermission",
                column: "permissionId",
                principalTable: "Permissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Permissions_ParentId",
                table: "Permissions",
                column: "ParentId",
                principalTable: "Permissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationRolePermission_Permissions_permissionId",
                table: "ApplicationRolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Permissions_ParentId",
                table: "Permissions");

            migrationBuilder.DropTable(
                name: "UserPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Permissions");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Permissions",
                newName: "parentid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Permissions",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Permissions",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Permissions",
                newName: "displayname");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_ParentId",
                table: "Permissions",
                newName: "IX_Permissions_parentid");

            migrationBuilder.AddColumn<int>(
                name: "permissionID",
                table: "Permissions",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "ApplicationUserRole",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "permissionID");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRolePermission_Permissions_permissionId",
                table: "ApplicationRolePermission",
                column: "permissionId",
                principalTable: "Permissions",
                principalColumn: "permissionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Permissions_parentid",
                table: "Permissions",
                column: "parentid",
                principalTable: "Permissions",
                principalColumn: "permissionID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
