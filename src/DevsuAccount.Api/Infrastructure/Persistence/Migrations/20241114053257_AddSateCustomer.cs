using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevsuAccount.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSateCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "State",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Customers");
        }
    }
}
