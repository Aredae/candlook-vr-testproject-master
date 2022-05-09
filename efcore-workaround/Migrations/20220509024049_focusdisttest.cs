using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace efcore_workaround.Migrations
{
    public partial class focusdisttest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<float>>(
                name: "approx_focus_dist",
                table: "recordings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "approx_focus_dist",
                table: "recordings");
        }
    }
}
