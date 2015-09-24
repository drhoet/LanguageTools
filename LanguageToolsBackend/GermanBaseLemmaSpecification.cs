using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class GermanBaseLemmaSpecification : ISqlSpecification<Lemma> {
        public string SearchFor { get; private set; }

        public string Sql { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

        public GermanBaseLemmaSpecification(string searchFor) {
            SearchFor = searchFor;
            Sql = "word=@word";
            Parameters = new Dictionary<string, object>();
            StringBuilder sb = new StringBuilder();
            List<string> queryParts = new List<String>();
            if(searchFor.EndsWith("es")) {
                queryParts.Add("((gender='m' or gender='n') and word || 'es'=@word)");
            }
            if(searchFor.EndsWith("s")) {
                queryParts.Add("((gender='m' or gender='n') and word || 's'=@word)");
            }
            queryParts.Add("(word=@word)");
            Sql = string.Join(" or ", queryParts);
            Parameters.Add("@word", searchFor);
        }

        public bool IsSatisfiedBy(Lemma entity) {
            if(entity == null) {
                return false;
            }
            switch(entity.Gender) {
                case Lemma.WordGender.Mannlich:
                case Lemma.WordGender.Neutrum:
                    List<string> baseWords = RemoveGermanGenitivInflictions(SearchFor);
                    foreach(string baseWord in baseWords) {
                        if(GermanEqualsIgnoreCase(entity.Word, baseWord)) {
                            return true;
                        }
                    }
                    return false;
                default:
                    return GermanEqualsIgnoreCase(entity.Word, SearchFor);
            }
        }

        private bool GermanEqualsIgnoreCase(string str1, string str2) {
            return CultureInfo.GetCultureInfo("de-DE").CompareInfo.Compare(str1, str2, CompareOptions.IgnoreCase) == 0;
        }

        /// <summary>
        /// Removes the Male and Neutral german inflictions from Genitiv. Plural is too hard to remove, since there are so many variants
        /// </summary>
        /// <param name="str"></param>
        /// <returns>A list of possible word bases</returns>
        private List<string> RemoveGermanGenitivInflictions(string str) {
            List<string> result = new List<string>();
            if(str.EndsWith("es")) {
                result.Add(str.Remove(str.Length - 2));
            }
            if(str.EndsWith("s")) {
                result.Add(str.Remove(str.Length - 1));
            }
            result.Add(str);
            return result;
        }
    }
}