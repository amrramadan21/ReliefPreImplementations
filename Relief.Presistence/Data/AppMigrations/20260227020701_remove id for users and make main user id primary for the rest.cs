using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relief.Presistence.Data.AppMigrations
{
    /// <inheritdoc />
    public partial class removeidforusersandmakemainuseridprimaryfortherest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CareHomeUsers_AspNetUsers_ApplicationUserId",
                table: "CareHomeUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualUsers_AspNetUsers_ApplicationUserId",
                table: "IndividualUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_PswUsers_AspNetUsers_ApplicationUserId",
                table: "PswUsers");

            migrationBuilder.DropIndex(
                name: "IX_PswUsers_ApplicationUserId",
                table: "PswUsers");

            migrationBuilder.DropIndex(
                name: "IX_IndividualUsers_ApplicationUserId",
                table: "IndividualUsers");

            migrationBuilder.DropIndex(
                name: "IX_CareHomeUsers_ApplicationUserId",
                table: "CareHomeUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "PswUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "IndividualUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "CareHomeUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_CareHomeUsers_AspNetUsers_Id",
                table: "CareHomeUsers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualUsers_AspNetUsers_Id",
                table: "IndividualUsers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PswUsers_AspNetUsers_Id",
                table: "PswUsers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CareHomeUsers_AspNetUsers_Id",
                table: "CareHomeUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualUsers_AspNetUsers_Id",
                table: "IndividualUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_PswUsers_AspNetUsers_Id",
                table: "PswUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "PswUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "IndividualUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "CareHomeUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PswUsers_ApplicationUserId",
                table: "PswUsers",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndividualUsers_ApplicationUserId",
                table: "IndividualUsers",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CareHomeUsers_ApplicationUserId",
                table: "CareHomeUsers",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CareHomeUsers_AspNetUsers_ApplicationUserId",
                table: "CareHomeUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualUsers_AspNetUsers_ApplicationUserId",
                table: "IndividualUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PswUsers_AspNetUsers_ApplicationUserId",
                table: "PswUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
