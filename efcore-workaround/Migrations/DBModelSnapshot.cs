﻿// <auto-generated />
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Util;

namespace efcore_workaround.Migrations
{
    [DbContext(typeof(DB))]
    partial class DBModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Util.Model.Game", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<long>("Version")
                        .HasColumnName("version")
                        .HasColumnType("bigint");

                    b.HasKey("Name", "Version")
                        .HasName("pk_games");

                    b.ToTable("games");
                });

            modelBuilder.Entity("Util.Model.Recording", b =>
                {
                    b.Property<int>("RecordingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("recording_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("GameName")
                        .HasColumnName("game_name")
                        .HasColumnType("text");

                    b.Property<long?>("GameVersion")
                        .HasColumnName("game_version")
                        .HasColumnType("bigint");

                    b.Property<List<float>>("LeftEyePosX")
                        .HasColumnName("left_eye_pos_x")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("LeftEyePosY")
                        .HasColumnName("left_eye_pos_y")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("LeftEyePosZ")
                        .HasColumnName("left_eye_pos_z")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("LeftGazeDirRelX")
                        .HasColumnName("left_gaze_dir_rel_x")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("LeftGazeDirRelY")
                        .HasColumnName("left_gaze_dir_rel_y")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("LeftGazeDirRelZ")
                        .HasColumnName("left_gaze_dir_rel_z")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("LeftGazeDirX")
                        .HasColumnName("left_gaze_dir_x")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("LeftGazeDirY")
                        .HasColumnName("left_gaze_dir_y")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("LeftGazeDirZ")
                        .HasColumnName("left_gaze_dir_z")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightEyePosX")
                        .HasColumnName("right_eye_pos_x")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightEyePosY")
                        .HasColumnName("right_eye_pos_y")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightEyePosZ")
                        .HasColumnName("right_eye_pos_z")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightGazeDirRelX")
                        .HasColumnName("right_gaze_dir_rel_x")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightGazeDirRelY")
                        .HasColumnName("right_gaze_dir_rel_y")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightGazeDirRelZ")
                        .HasColumnName("right_gaze_dir_rel_z")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightGazeDirX")
                        .HasColumnName("right_gaze_dir_x")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightGazeDirY")
                        .HasColumnName("right_gaze_dir_y")
                        .HasColumnType("real[]");

                    b.Property<List<float>>("RightGazeDirZ")
                        .HasColumnName("right_gaze_dir_z")
                        .HasColumnType("real[]");

                    b.Property<int>("Subject_id")
                        .HasColumnName("subject_id")
                        .HasColumnType("integer");

                    b.Property<List<long>>("TimestampNS")
                        .HasColumnName("timestamp_ns")
                        .HasColumnType("bigint[]");

                    b.HasKey("RecordingId")
                        .HasName("pk_recordings");

                    b.HasIndex("GameName", "GameVersion")
                        .HasName("ix_recordings_game_name_game_version");

                    b.ToTable("recordings");
                });

            modelBuilder.Entity("Util.Model.Recording", b =>
                {
                    b.HasOne("Util.Model.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameName", "GameVersion")
                        .HasConstraintName("fk_recordings_games_game_name_game_version");
                });
#pragma warning restore 612, 618
        }
    }
}
