using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace Util
{
    // Running migrations:
    //
    // EFCore does not support automatic migrations, for a number of good reasons
    // (https://github.com/dotnet/efcore/issues/6214#issuecomment-674160148).
    // Instead, we invoke a tool that automatically creates migration files when
    // we are ready to do so and then run these migrations on the database. This
    // workflow is comparable to Django and other web frameworks.
    // Unity does not really integrate well with NuGet package management and the
    // ways of modern VS project layout. To get these automated tools working, we
    // have to rely on a fairly ugly hack (trust me, I've tried everything...).
    // Please read <project-root>/efcore-workaround/README.md to learn more.
    //
    // Command summary (read the above first!):
    // - To create a new migration, run
    //     Add-Migration <Name>
    // - To revert the last migration, run
    //     Remove-Migration [ -Force ]
    //   Use -Force to revert a migration that was already applied to the DB
    // - To apply all pending migrations to the DB, run
    //     Update-Database

    public class DB : DbContext
    {
        public DbSet<Model.Recording> Recordings { get; set; }
        public DbSet<Model.Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
            .UseNpgsql("Host=localhost;Database=playground;Username=edugamelab;Password=edugame")
            .UseSnakeCaseNamingConvention();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .Entity<Model.Game>()
                .HasKey(game => new { game.Name, game.Version }); // declare composite key
    }

    namespace Model
    {
        public class Recording
        {
            public int RecordingId { get; set; }
            public Game Game { get; set; }
            public int Subject_id { get; set; }
            // These lists will be translated by Npgsql to use PostgreSQL arrays, which can be efficiently
            // read by psycopg2 into numpy arrays for data analysis. This is considerably more efficient
            // than using a single row for every entry.

            // timestamp in nanoseconds
            public List<long> TimestampNS { get; set; } = new List<long>();
            // world-space
            public List<float> LeftEyePosX { get; set; } = new List<float>();
            public List<float> LeftEyePosY { get; set; } = new List<float>();
            public List<float> LeftEyePosZ { get; set; } = new List<float>();
            public List<float> RightEyePosX { get; set; } = new List<float>();
            public List<float> RightEyePosY { get; set; } = new List<float>();
            public List<float> RightEyePosZ { get; set; } = new List<float>();
            // world-space
            public List<float> LeftGazeDirX { get; set; } = new List<float>();
            public List<float> LeftGazeDirY { get; set; } = new List<float>();
            public List<float> LeftGazeDirZ { get; set; } = new List<float>();
            public List<float> RightGazeDirX { get; set; } = new List<float>();
            public List<float> RightGazeDirY { get; set; } = new List<float>();
            public List<float> RightGazeDirZ { get; set; } = new List<float>();
            // head-pose relative
            public List<float> LeftGazeDirRelX { get; set; } = new List<float>();
            public List<float> LeftGazeDirRelY { get; set; } = new List<float>();
            public List<float> LeftGazeDirRelZ { get; set; } = new List<float>();
            public List<float> RightGazeDirRelX { get; set; } = new List<float>();
            public List<float> RightGazeDirRelY { get; set; } = new List<float>();
            public List<float> RightGazeDirRelZ { get; set; } = new List<float>();

            public void addEyeData(EyeData eyeData)
            {
                this.TimestampNS.Add(eyeData.timestamp_ns);
                this.LeftEyePosX.Add(eyeData.left.position.x);
                this.LeftEyePosY.Add(eyeData.left.position.y);
                this.LeftEyePosZ.Add(eyeData.left.position.z);
                this.RightEyePosX.Add(eyeData.right.position.x);
                this.RightEyePosY.Add(eyeData.right.position.y);
                this.RightEyePosZ.Add(eyeData.right.position.z);
                this.LeftGazeDirX.Add(eyeData.left.gazeDirection.x);
                this.LeftGazeDirY.Add(eyeData.left.gazeDirection.y);
                this.LeftGazeDirZ.Add(eyeData.left.gazeDirection.z);
                this.RightGazeDirX.Add(eyeData.right.gazeDirection.x);
                this.RightGazeDirY.Add(eyeData.right.gazeDirection.y);
                this.RightGazeDirZ.Add(eyeData.right.gazeDirection.z);
                this.LeftGazeDirRelX.Add(eyeData.left.gazeDirectionRel.x);
                this.LeftGazeDirRelY.Add(eyeData.left.gazeDirectionRel.y);
                this.LeftGazeDirRelZ.Add(eyeData.left.gazeDirectionRel.z);
                this.RightGazeDirRelX.Add(eyeData.right.gazeDirectionRel.x);
                this.RightGazeDirRelY.Add(eyeData.right.gazeDirectionRel.y);
                this.RightGazeDirRelZ.Add(eyeData.right.gazeDirectionRel.z);
            }
            public void addEyeData(IEnumerable<EyeData> data)
            {
                this.TimestampNS.AddRange(data.Select(eyeData => eyeData.timestamp_ns));
                this.LeftEyePosX.AddRange(data.Select(eyeData => eyeData.left.position.x));
                this.LeftEyePosY.AddRange(data.Select(eyeData => eyeData.left.position.y));
                this.LeftEyePosZ.AddRange(data.Select(eyeData => eyeData.left.position.z));
                this.RightEyePosX.AddRange(data.Select(eyeData => eyeData.right.position.x));
                this.RightEyePosY.AddRange(data.Select(eyeData => eyeData.right.position.y));
                this.RightEyePosZ.AddRange(data.Select(eyeData => eyeData.right.position.z));
                this.LeftGazeDirX.AddRange(data.Select(eyeData => eyeData.left.gazeDirection.x));
                this.LeftGazeDirY.AddRange(data.Select(eyeData => eyeData.left.gazeDirection.y));
                this.LeftGazeDirZ.AddRange(data.Select(eyeData => eyeData.left.gazeDirection.z));
                this.RightGazeDirX.AddRange(data.Select(eyeData => eyeData.right.gazeDirection.x));
                this.RightGazeDirY.AddRange(data.Select(eyeData => eyeData.right.gazeDirection.y));
                this.RightGazeDirZ.AddRange(data.Select(eyeData => eyeData.right.gazeDirection.z));
                this.LeftGazeDirRelX.AddRange(data.Select(eyeData => eyeData.left.gazeDirectionRel.x));
                this.LeftGazeDirRelY.AddRange(data.Select(eyeData => eyeData.left.gazeDirectionRel.y));
                this.LeftGazeDirRelZ.AddRange(data.Select(eyeData => eyeData.left.gazeDirectionRel.z));
                this.RightGazeDirRelX.AddRange(data.Select(eyeData => eyeData.right.gazeDirectionRel.x));
                this.RightGazeDirRelY.AddRange(data.Select(eyeData => eyeData.right.gazeDirectionRel.y));
                this.RightGazeDirRelZ.AddRange(data.Select(eyeData => eyeData.right.gazeDirectionRel.z));
            }
        }

        public class Game
        {
            public string Name { get; set; }
            public uint Version { get; set; }

            public override bool Equals(object obj)
            {
                return obj is Game game &&
                       Name == game.Name &&
                       Version == game.Version;
            }

            public override int GetHashCode()
            {
                return System.HashCode.Combine(Name, Version);
            }

            public override string ToString()
            {
                return Name + "-v" + Version;
            }
        }
    }

}
