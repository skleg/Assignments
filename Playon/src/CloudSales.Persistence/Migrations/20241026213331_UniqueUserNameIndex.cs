using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSales.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueUserNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserName",
                table: "Customers",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_UserName",
                table: "Customers");
        }
    }
}
