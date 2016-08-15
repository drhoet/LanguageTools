using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class ExactLemmaSpecification : ISqlSpecification<Noun> {
        public string SearchFor { get; private set; }

        public string Sql { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

        public ExactLemmaSpecification(string searchFor) {
            SearchFor = searchFor;
            Sql = "word=@word";
            Parameters = new Dictionary<string, object>();
            Parameters.Add("@word", SearchFor);
        }

        public bool IsSatisfiedBy(Noun entity) {
            return entity != null && CultureInfo.GetCultureInfo("de-DE").CompareInfo.Compare(entity.Lemma, SearchFor, CompareOptions.IgnoreCase) == 0;
        }
    }
}