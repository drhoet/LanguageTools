using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace LanguageTools.Backend
{
    public class PrepositionRepository : LemmaRepository<Preposition>
    {
        public PrepositionRepository(LemmaDatabase db) : base(db, "preposition", new string[] { "wordCase" })
        {
        }

        protected internal override Preposition ConstructFromRecord(DbDataReader reader)
        {
            Preposition l = new Preposition();
            l.Id = reader.GetInt32(0);
            l.Word = reader.GetString(1);
            l.PrepositionCase = PrepositionCaseConvert.ToCase(reader.GetString(2));
            return l;
        }

        protected internal override object[] ConstructRecordFrom(Preposition val)
        {
            return new object[] { PrepositionCaseConvert.ToString(val.PrepositionCase) };
        }

    }
}
