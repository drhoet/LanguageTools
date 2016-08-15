﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LanguageTools.Backend
{
    public abstract class LemmaRepository<T> : IPaginated<T> where T : Lemma
    {
        protected internal LemmaDatabase Database { get; private set; }

        protected internal DbCommand InsertCmd { get; }
        protected internal DbCommand UpdateCmd { get; }
        protected internal DbCommand DeleteCmd { get; }
        protected internal DbCommand CountCmd { get; }
        protected internal DbCommand SelectByIdCmd { get; }
        protected internal DbCommand SelectPageOfDataCmd { get; }

        protected internal string TableName { get; }
        private string[] ColumNames;
        
        public LemmaRepository(LemmaDatabase db, string tableName, string[] columnNames)
        {
            Database = db;
            TableName = tableName;
            ColumNames = columnNames;

            InsertCmd = db.CreateCommand("insert into " + TableName + "(word," + string.Join(",", columnNames)
                + ") values(@word, @" + string.Join(", @", columnNames) + ")");
            InsertCmd.Prepare();
            string[] columnSetters = columnNames.Select(x => x + "=@" + x).ToArray(); // x=@x
            UpdateCmd = db.CreateCommand("update " + TableName + " set word=@word, " + string.Join(", ", columnSetters) + " where id=@id");
            UpdateCmd.Prepare();
            DeleteCmd = db.CreateCommand("delete from " + TableName + " where id=@id");
            DeleteCmd.Prepare();
            CountCmd = db.CreateCommand("select count(*) from " + TableName);
            CountCmd.Prepare();
            SelectByIdCmd = db.CreateCommand("select * from " + TableName + " where id=@id");
            SelectByIdCmd.Prepare();
            SelectPageOfDataCmd = db.CreateCommand("select * from " + TableName + " limit @pageSize offset @offset");
            SelectPageOfDataCmd.Prepare();
        }

        protected internal abstract T ConstructFromRecord(DbDataReader reader);
        protected internal abstract object[] ConstructRecordFrom(T val);

        public int Count
        {
            get
            {
                return Convert.ToInt32(CountCmd.ExecuteScalar());
            }
        }

        public T GetById(int id)
        {
            SelectByIdCmd.Parameters.Add(Database.CreateParameter("@id", id, SelectByIdCmd));
            using (DbDataReader reader = SelectByIdCmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (reader.Read())
                {
                    return ConstructFromRecord(reader);
                }
                else
                {
                    return null;
                }
            }
        }

        public T FindOne(ISqlSpecification<T> spec)
        {
            using (DbDataReader reader = Database.ExecuteReader("select * from " + TableName + " where " + spec.Sql, spec.Parameters,
                CommandBehavior.SingleRow))
            {
                if (reader.Read())
                {
                    return ConstructFromRecord(reader);
                }
                else
                {
                    return null;
                }
            }
        }

        public List<T> FindAll(ISqlSpecification<T> spec)
        {
            List<T> list = new List<T>();
            using (DbDataReader reader = Database.ExecuteReader("select * from " + TableName + " where " + spec.Sql, spec.Parameters,
                CommandBehavior.Default))
            {
                while (reader.Read())
                {
                    list.Add(ConstructFromRecord(reader));
                }
            }
            return list;
        }

        /// <summary>
        /// Adds the given Lemma to the repository. The Id of the lemma is autogenerated.
        /// </summary>
        /// <param name="val"></param>
        public void Add(T val)
        {
            InsertCmd.Parameters.Add(Database.CreateParameter("@word", val.Word, InsertCmd));
            object[] rec = ConstructRecordFrom(val);
            for (int i = 0; i < ColumNames.Count(); ++i)
            {
                InsertCmd.Parameters.Add(Database.CreateParameter("@" + ColumNames[i], rec[i], InsertCmd));
            }
            int id = InsertCmd.ExecuteNonQuery();
            val.Id = id;
        }

        /// <summary>
        /// Update the given Lemma in the repository. The Lemma must already exist!!
        /// </summary>
        /// <param name="val"></param>
        public void Update(T val)
        {
            UpdateCmd.Parameters.Add(Database.CreateParameter("@id", val.Id, UpdateCmd));
            UpdateCmd.Parameters.Add(Database.CreateParameter("@word", val.Word, UpdateCmd));
            object[] rec = ConstructRecordFrom(val);
            for (int i = 0; i < ColumNames.Count(); ++i)
            {
                UpdateCmd.Parameters.Add(Database.CreateParameter("@" + ColumNames[i], rec[i], UpdateCmd));
            }
            UpdateCmd.ExecuteNonQuery();
        }

        public void RemoveById(int id)
        {
            DeleteCmd.Parameters.Add(Database.CreateParameter("id", id, DeleteCmd));
            DeleteCmd.ExecuteNonQuery();
        }

        public void Remove(T l)
        {
            RemoveById(l.Id);
        }

        public List<T> SupplyPageOfData(int pageIndex, int pageSize)
        {
            List<T> result = new List<T>();
            // Retrieve the specified number of rows from the database, starting
            // with the row specified by the lowerPageBoundary parameter.
            SelectPageOfDataCmd.Parameters.Add(Database.CreateParameter("@pageSize", pageSize, SelectPageOfDataCmd));
            SelectPageOfDataCmd.Parameters.Add(Database.CreateParameter("@offset", pageIndex * pageSize, SelectPageOfDataCmd));
            using (DbDataReader reader = SelectPageOfDataCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(ConstructFromRecord(reader));
                }
            }
            return result;
        }

    }
}