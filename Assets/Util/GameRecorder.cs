using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Assertions;

namespace Util
{
    sealed class GameRecorder : IDisposable
    {
        private Model.Game game;
        private EyeTracker et;
        private DB db;
        private Model.Recording recording;
        private bool disposed = false;
        private bool committed = false;

        public GameRecorder(Model.Game game, EyeTracker et)
        {
            this.et = et;
            this.game = game;
            this.db = new DB();
            bool done = false;
            int count = 0;
            while (!done) // a kind of test-and-test-and-set lock, i guess
            {
                using var tx = db.Database.BeginTransaction();
                Model.Game db_game = db.Games.Find(game.Name, game.Version);
                if (db_game == null)
                {
                    db.Games.Add(game);
                    db.SaveChanges();
                } else
                {
                    game = db_game;
                }
                try
                {
                    tx.Commit();
                    done = true;
                } catch (Exception)
                {
                    tx.Rollback();
                    // realistically, this can only fail if the game has since been added:
                    done = db.Games.Any(g => g.Equals(game));
                    count += 1;
                    if (count >= 5)
                    {
                        throw new Exception("Failed to commit new game to database for 5 tries, giving up.");
                    }
                }
            }
            this.recording = new Model.Recording { Game = game };
        }

        // KLUDGE: must be called every frame => couples tightly to implementation of EyeTracker
        public void Update()
        {
            Assert.IsFalse(this.committed);
            Assert.IsFalse(this.disposed);
            List<EyeData> freshData = et.getFreshData();
            this.recording.addEyeData(freshData);
        }

        public void Commit()
        {
            db.Recordings.Add(this.recording);
            db.SaveChanges();
            this.committed = true;
        }

        void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.db.Dispose();
                    if (!this.committed)
                    {
                        Debug.LogWarning("Disposing recorded ET data without comitting to database.\n" +
                            "This could be a programming error.");
                    }
                }

                disposed = true;
                this.db = null;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
