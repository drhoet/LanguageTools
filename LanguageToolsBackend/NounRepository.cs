using System;
using System.Collections.Generic;
using System.Data.Common;

namespace LanguageTools.Backend
{
    public class NounRepository: LemmaRepository<Noun>
    {
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
    }
}
