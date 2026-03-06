using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relief.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class removingrequeststatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "JopRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "JobOfferId1",
                table: "JopRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VaccinationPolicy",
                table: "CareHomeUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_JopRequests_JobOfferId1",
                table: "JopRequests",
                column: "JobOfferId1");

            migrationBuilder.AddForeignKey(
                name: "FK_JopRequests_JobOffers_JobOfferId1",
                table: "JopRequests",
                column: "JobOfferId1",
                principalTable: "JobOffers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JopRequests_JobOffers_JobOfferId1",
                table: "JopRequests");

            migrationBuilder.DropIndex(
                name: "IX_JopRequests_JobOfferId1",
                table: "JopRequests");

            migrationBuilder.DropColumn(
                name: "JobOfferId1",
                table: "JopRequests");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "JopRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "VaccinationPolicy",
                table: "CareHomeUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
