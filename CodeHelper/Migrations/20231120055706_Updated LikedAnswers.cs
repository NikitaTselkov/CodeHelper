using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHelper.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLikedAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Like_Answers_AnswerId",
                table: "Like");

            migrationBuilder.DropIndex(
                name: "IX_Like_AnswerId",
                table: "Like");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Like_AnswerId",
                table: "Like",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Answers_AnswerId",
                table: "Like",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
