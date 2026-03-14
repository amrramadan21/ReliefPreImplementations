using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relief.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class editingstatusforpsw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "PswUsers");

            migrationBuilder.DropColumn(
                name: "WorkStatus",
                table: "PswUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "PswUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkStatus",
                table: "PswUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
