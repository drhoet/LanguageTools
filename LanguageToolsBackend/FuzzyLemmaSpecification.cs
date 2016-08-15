using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class FuzzyLemmaSpecification : ISqlSpecification<Noun> {
        public string SearchFor { get; private set; }

        public string Sql { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

        public FuzzyLemmaSpecification(string searchFor) {
            SearchFor = searchFor;
            Sql = "word like @word escape '^'";
            Parameters = new Dictionary<string, object>();
            Parameters.Add("@word", "%" + SearchFor.Replace("%", "^%").Replace("_", "^_").Replace("^", "^^") + "%");
        }

        public bool IsSatisfiedBy(Noun entity) {
            return entity != null && GermanEqualsIgnoreCase(entity.Word, SearchFor);
        }

        private bool GermanEqualsIgnoreCase(string str1, string str2) {
            return CultureInfo.GetCultureInfo("de-DE").CompareInfo.Compare(str1, str2, CompareOptions.IgnoreCase) == 0;
        }
    }
}