using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHelper.Migrations
{
    /// <inheritdoc />
    public partial class UpdatenAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptedAnswer",
                table: "Answers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptedAnswer",
                table: "Answers");
        }
    }
}
