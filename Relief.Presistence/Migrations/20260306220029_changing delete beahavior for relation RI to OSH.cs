using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relief.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class changingdeletebeahaviorforrelationRItoOSH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRequestItems_OfferShifts_ShiftId",
                table: "JobRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_JopRequests_JobOffers_JobOfferId1",
                table: "JopRequests");

            migrationBuilder.DropIndex(
                name: "IX_JopRequests_JobOfferId1",
                table: "JopRequests");

            migrationBuilder.DropColumn(
                name: "JobOfferId1",
                table: "JopRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequestItems_OfferShifts_ShiftId",
                table: "JobRequestItems",
                column: "ShiftId",
                principalTable: "OfferShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRequestItems_OfferShifts_ShiftId",
                table: "JobRequestItems");

            migrationBuilder.AddColumn<Guid>(
                name: "JobOfferId1",
                table: "JopRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JopRequests_JobOfferId1",
                table: "JopRequests",
                column: "JobOfferId1");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequestItems_OfferShifts_ShiftId",
                table: "JobRequestItems",
                column: "ShiftId",
                principalTable: "OfferShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JopRequests_JobOffers_JobOfferId1",
                table: "JopRequests",
                column: "JobOfferId1",
                principalTable: "JobOffers",
                principalColumn: "Id");
        }
    }
}
