using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relief.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class changingofferstructureandremovepswcompleteprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProfileCompleted",
                table: "PswUsers");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "JobOffers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "JobOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "JobOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "JobOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Preferences",
                table: "JobOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "JobOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address2",
                table: "JobOffers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "JobOffers");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "JobOffers");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "JobOffers");

            migrationBuilder.DropColumn(
                name: "Preferences",
                table: "JobOffers");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "JobOffers");

            migrationBuilder.AddColumn<bool>(
                name: "IsProfileCompleted",
                table: "PswUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
