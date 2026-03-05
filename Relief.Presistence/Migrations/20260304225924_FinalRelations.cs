using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relief.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class FinalRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OfferShifts_AssignedPswId",
                table: "OfferShifts",
                column: "AssignedPswId");

            migrationBuilder.CreateIndex(
                name: "IX_JopRequests_JobOfferId",
                table: "JopRequests",
                column: "JobOfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_JopRequests_JobOffers_JobOfferId",
                table: "JopRequests",
                column: "JobOfferId",
                principalTable: "JobOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferShifts_PswUsers_AssignedPswId",
                table: "OfferShifts",
                column: "AssignedPswId",
                principalTable: "PswUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JopRequests_JobOffers_JobOfferId",
                table: "JopRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferShifts_PswUsers_AssignedPswId",
                table: "OfferShifts");

            migrationBuilder.DropIndex(
                name: "IX_OfferShifts_AssignedPswId",
                table: "OfferShifts");

            migrationBuilder.DropIndex(
                name: "IX_JopRequests_JobOfferId",
                table: "JopRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Addresses");
        }
    }
}
