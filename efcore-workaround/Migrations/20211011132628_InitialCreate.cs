using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace efcore_workaround.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    name = table.Column<string>(nullable: false),
                    version = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => new { x.name, x.version });
                });

            migrationBuilder.CreateTable(
                name: "recordings",
                columns: table => new
                {
                    recording_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    game_name = table.Column<string>(nullable: true),
                    game_version = table.Column<long>(nullable: true),
                    subject_id = table.Column<int>(nullable: true),
                    timestamp_ns = table.Column<List<long>>(nullable: true),
                    left_eye_pos_x = table.Column<List<float>>(nullable: true),
                    left_eye_pos_y = table.Column<List<float>>(nullable: true),
                    left_eye_pos_z = table.Column<List<float>>(nullable: true),
                    right_eye_pos_x = table.Column<List<float>>(nullable: true),
                    right_eye_pos_y = table.Column<List<float>>(nullable: true),
                    right_eye_pos_z = table.Column<List<float>>(nullable: true),
                    left_gaze_dir_x = table.Column<List<float>>(nullable: true),
                    left_gaze_dir_y = table.Column<List<float>>(nullable: true),
                    left_gaze_dir_z = table.Column<List<float>>(nullable: true),
                    right_gaze_dir_x = table.Column<List<float>>(nullable: true),
                    right_gaze_dir_y = table.Column<List<float>>(nullable: true),
                    right_gaze_dir_z = table.Column<List<float>>(nullable: true),
                    left_gaze_dir_rel_x = table.Column<List<float>>(nullable: true),
                    left_gaze_dir_rel_y = table.Column<List<float>>(nullable: true),
                    left_gaze_dir_rel_z = table.Column<List<float>>(nullable: true),
                    right_gaze_dir_rel_x = table.Column<List<float>>(nullable: true),
                    right_gaze_dir_rel_y = table.Column<List<float>>(nullable: true),
                    right_gaze_dir_rel_z = table.Column<List<float>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recordings", x => x.recording_id);
                    table.ForeignKey(
                        name: "fk_recordings_games_game_name_game_version",
                        columns: x => new { x.game_name, x.game_version },
                        principalTable: "games",
                        principalColumns: new[] { "name", "version" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recordings_game_name_game_version",
                table: "recordings",
                columns: new[] { "game_name", "game_version" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recordings");

            migrationBuilder.DropTable(
                name: "games");
        }
    }
}
