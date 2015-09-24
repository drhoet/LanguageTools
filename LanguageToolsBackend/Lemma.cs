using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class WordGenderConvert {

        public static string ToString(Lemma.WordGender obj) {
            switch(obj) {
                case Lemma.WordGender.Mannlich: return "m";
                case Lemma.WordGender.Neutrum: return "n";
                case Lemma.WordGender.Weiblich: return "f";
                case Lemma.WordGender.Plural: return "pl";
                case Lemma.WordGender.Undefined: return "";
                default: throw new ArgumentException("Impossible value of WordGender received: " + obj);
            }
        }

        public static Lemma.WordGender ToGender(object obj) {
            string str = Convert.ToString(obj);
            switch(str) {
                case "m": return Lemma.WordGender.Mannlich;
                case "n": return Lemma.WordGender.Neutrum;
                case "f": return Lemma.WordGender.Weiblich;
                case "pl": return Lemma.WordGender.Plural;
                case "": return Lemma.WordGender.Undefined;
                default: throw new ArgumentException("Invalid value for WordGender: " + str);
            }
        }
    }

    public class Lemma {
        public enum WordGender { Mannlich, Weiblich, Neutrum, Plural, Undefined };

        public int Id { get; internal set; }
        public string Word { get; set; }
        public WordGender Gender { get; set; }

        public Lemma(string text, WordGender gender) {
            Word = text;
            Gender = gender;
        }

        public Lemma() {
            Word = "";
            Gender = WordGender.Undefined;
        }
    }
}