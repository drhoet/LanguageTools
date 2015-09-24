using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class ExactLemmaSpecification : ISqlSpecification<Lemma> {
        public string SearchFor { get; private set; }

        public string Sql { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

        public ExactLemmaSpecification(string searchFor) {
            SearchFor = searchFor;
            Sql = "word=@word";
            Parameters = new Dictionary<string, object>();
            Parameters.Add("@word", SearchFor);
        }

        public bool IsSatisfiedBy(Lemma entity) {
            return entity != null && CultureInfo.GetCultureInfo("de-DE").CompareInfo.Compare(entity.Word, SearchFor, CompareOptions.IgnoreCase) == 0;
        }
    }
}