using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final.Migrations
{
    /// <inheritdoc />
    public partial class fixAdminHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aea95e2c-558c-4d3d-9732-54aa224e8b19", "AQAAAAIAAYagAAAAED9StH9whnC7QfgmADpElvlBefHDfCSPSTcHiLs7xoNeC1iPgMup6pC9sYapK+VxkQ==", "444b5be5-98af-4ff9-b282-ce6e8cd2196f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "da4ee260-e06b-4a71-9dd9-83da235f1201", "AQAAAAEAACcQAAAAEL5faRHVlKvtUhDsRT4MeToo1UvJbLbKriVJA1QJqAHMb9Iih3YgN/XTOkm5RwZ7Uw==", "a9565b42-9a16-446d-a6f0-715082e47d6d" });
        }
    }
}
