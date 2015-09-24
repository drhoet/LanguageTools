using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class GermanCompositionEndLemmaSpecification : ISqlSpecification<Lemma> {
        public string SearchFor { get; private set; }

        public string Sql { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

        public GermanCompositionEndLemmaSpecification(string searchFor) {
            SearchFor = searchFor;
            Sql = "@text like  '%' || text order by length(text) desc";
            Parameters = new Dictionary<string, object>();
            Parameters.Add("@text", searchFor);
        }

        public bool IsSatisfiedBy(Lemma entity) {
            return entity != null && CultureInfo.GetCultureInfo("de-DE").CompareInfo.IsSuffix(SearchFor, entity.Text, CompareOptions.IgnoreCase);
        }

        private bool GermanEqualsIgnoreCase(string str1, string str2) {
            return CultureInfo.GetCultureInfo("de-DE").CompareInfo.Compare(str1, str2, CompareOptions.IgnoreCase) == 0;
        }
    }
}