using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Collections.Generic;

namespace efcore_workaround.Migrations
{
    public partial class TestWithUid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "subject_id",
                table: "recordings",
                nullable: true);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
