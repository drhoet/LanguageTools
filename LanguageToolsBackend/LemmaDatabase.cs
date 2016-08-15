using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace LanguageTools.Backend {
    public class LemmaDatabase {
        private DbTransaction tran = null;
        protected DbConnection conn { get; private set; }

        public static LemmaDatabase CreateDefaultInstance() {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = Path.Combine(folder, "LanguageTools");
            if(!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            return new LemmaDatabase(Path.Combine(folder, "default.sqlite"));
        }

        public LemmaDatabase(string fileName) {
            SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder();
            connBuilder.DataSource = fileName;
            connBuilder.Version = 3;

            conn = new SQLiteConnection(connBuilder.ToString());
            conn.Open();
            InitializeDatabase();
        }

        private void InitializeDatabase() {
            OpenChangeSet();
            ExecuteNonQuery("create table if not exists lemma (id integer primary key not null, word text collate nocase, gender text)", null);
            ExecuteNonQuery("create index if not exists idx_lemma_word on lemma(word)", null);
            ExecuteNonQuery("create index if not exists idx_lemma_gender_word on lemma(gender, word)", null);
            ExecuteNonQuery("create table if not exists lemma_meaning(lemma_id integer, meaning text, foreign key(lemma_id) references lemma(id))", null);
            ExecuteNonQuery("create table if not exists preposition (id integer primary key not null, word text collate nocase, wordCase text)", null);

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
            return CreateCommand(commandText, null);
        }

        internal DbCommand CreateCommand(string commandText, Dictionary<string, object> parameters) {
            DbCommand result = conn.CreateCommand();
            result.CommandText = commandText;
            if(parameters != null) {
                foreach(KeyValuePair<string, object> param in parameters) {
                    result.Parameters.Add(CreateParameter(param.Key, param.Value, result));
                }
            }
            return result;
        }

        internal DbParameter CreateParameter(string name, object value, DbCommand command) {
            DbParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            return param;
        }

        public int ExecuteNonQuery(string queryText, Dictionary<string, object> parameters) {
            return CreateCommand(queryText, parameters).ExecuteNonQuery();
        }

        internal DbDataReader ExecuteReader(string queryText, Dictionary<string, object> parameters, CommandBehavior options) {
            return CreateCommand(queryText, parameters).ExecuteReader(options);
        }

        /// <summary>
        /// Enabled super-fast mode. This writes the journal only to memory, meaning in case of app-crash, you lose everything.
        /// </summary>
        public void EnableFastInsertMode()
        {
            ExecuteNonQuery("PRAGMA synchronous = OFF", null);
            ExecuteNonQuery("PRAGMA journal_mode = MEMORY", null);
        }

    }
}