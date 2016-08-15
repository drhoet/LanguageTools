using System;
using System.Collections.Generic;
using System.Data.Common;

namespace LanguageTools.Backend
{
    public class NounRepository: LemmaRepository<Noun>
    {
        public struct BulkItem
        {
            public string Word;
            public string Gender;
        }

        public NounRepository(LemmaDatabase db): base(db, "lemma", new string[] { "gender" })
        {
        }

        protected internal override Noun ConstructFromRecord(DbDataReader reader)
        {
            Noun l = new Noun();
            l.Id = reader.GetInt32(0);
            l.Word = reader.GetString(1);
            l.Gender = NounGenderConvert.ToGender(reader.GetString(2));
            return l;
        }

        protected internal override object[] ConstructRecordFrom(Noun val)
        {
            return new object[] { NounGenderConvert.ToString(val.Gender) };
        }

        /// <summary>
        /// Adds a list of items, provided as an array of structs. This method does no validation at all, and is supposed to be used
        /// for performance reasons only.
        /// </summary>
        /// <param name="items"></param>
        public void AddBulk(IEnumerable<BulkItem> items)
        {
            foreach (BulkItem item in items)
            {
                InsertCmd.Parameters.Add(Database.CreateParameter("@word", item.Word, InsertCmd));
                InsertCmd.Parameters.Add(Database.CreateParameter("@gender", item.Gender, InsertCmd));
                InsertCmd.ExecuteNonQuery();
            }
        }

    }
}
