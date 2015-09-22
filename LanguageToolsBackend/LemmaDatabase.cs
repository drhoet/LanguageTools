using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class LemmaDatabase {
        private DbTransaction tran = null;
        protected DbConnection conn { get; private set; }

        public LemmaDatabase(string connectionString) {
            conn = new SQLiteConnection(connectionString);
            conn.Open();
            InitializeDatabase();
        }

        private void InitializeDatabase() {
            OpenChangeSet();
            DbCommand command = CreateCommand("create table if not exists lemma (id integer primary key not null, text varchar(100), gender varchar(2))");
            command.ExecuteNonQuery();
            CommitChangeSet();
        }

        public void CloseDatabase() {
            if(tran != null) {
                tran.Rollback();
            }
            conn.Close();
        }

        /// <summary>
        /// Opens a change set. From now on, changes will not be persisted immediately.
        /// </summary>
        public void OpenChangeSet() {
            if(tran != null) {
                throw new InvalidOperationException("A changeset is already open.");
            }
            tran = conn.BeginTransaction();
        }

        /// <summary>
        /// Saves all changes in the open change set to the database.
        /// </summary>
        public void CommitChangeSet() {
            if(tran == null) {
                throw new InvalidOperationException("No open change set to commit.");
            }
            tran.Commit();
            tran = null;
        }

        /// <summary>
        /// Rolls back all changes in the change set.
        /// </summary>
        public void RollBackChangeSet() {
            if(tran == null) {
                throw new InvalidOperationException("No open change set to roll back.");
            }
            tran.Rollback();
            tran = null;
        }

        internal DbCommand CreateCommand(string commandText) {
            DbCommand result = conn.CreateCommand();
            result.CommandText = commandText;
            return result;
        }

        internal DbParameter CreateParameter(string name, object value, DbCommand command) {
            DbParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            return param;
        }

        public int ExecuteNonQuery(string queryText) {
            return CreateCommand(queryText).ExecuteNonQuery();
        }
    }
}