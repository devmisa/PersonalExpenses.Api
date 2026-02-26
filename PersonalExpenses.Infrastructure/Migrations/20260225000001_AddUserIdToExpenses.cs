using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalExpenses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses",
                column: "UserId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses");

            _ = migrationBuilder.DropIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses");

            _ = migrationBuilder.DropColumn(
                name: "UserId",
                table: "Expenses");
        }
    }
}
