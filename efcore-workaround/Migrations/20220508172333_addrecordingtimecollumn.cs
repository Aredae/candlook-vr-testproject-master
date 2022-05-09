using Microsoft.EntityFrameworkCore.Migrations;

namespace efcore_workaround.Migrations
{
    public partial class addrecordingtimecollumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
           migrationBuilder.AddColumn<string>(
                name: "recordingtime",
                table: "recordings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
