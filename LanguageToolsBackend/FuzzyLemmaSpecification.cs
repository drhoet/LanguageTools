using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class FuzzyLemmaSpecification : ISqlSpecification<Lemma> {
        public string SearchFor { get; private set; }

        public Dictionary<string, object> Parameters { get; private set; }

        public FuzzyLemmaSpecification(string searchFor) {
            SearchFor = searchFor;
            Parameters = new Dictionary<string, object>();
            Parameters.Add("text", SearchFor);
        }

        public bool IsSatisfiedBy(Lemma entity) {
            return entity != null && entity.Text.Contains(SearchFor);
        }

        public string ToSqlString() {
            return "text like %@text%";
        }
    }
}