using System;

namespace LanguageTools.Backend {
    public class WordGenderConvert {

        public static string ToString(Noun.NounGender obj) {
            switch(obj) {
                case Noun.NounGender.Mannlich: return "m";
                case Noun.NounGender.Neutrum: return "n";
                case Noun.NounGender.Weiblich: return "f";
                case Noun.NounGender.Plural: return "pl";
                case Noun.NounGender.Singular: return "sg";
                case Noun.NounGender.Undefined: return "";
                default: throw new ArgumentException("Impossible value of NounGender received: " + obj);
            }
        }

        public static Noun.NounGender ToGender(object obj) {
            string str = Convert.ToString(obj);
            switch(str) {
                case "m": return Noun.NounGender.Mannlich;
                case "n": return Noun.NounGender.Neutrum;
                case "f": return Noun.NounGender.Weiblich;
                case "pl": return Noun.NounGender.Plural;
                case "sg": return Noun.NounGender.Singular;
                case "": return Noun.NounGender.Undefined;
                default: throw new ArgumentException("Invalid value for NounGender: " + str);
            }
        }
    }

    public class Noun {
        public enum NounGender { Mannlich, Weiblich, Neutrum, Singular, Plural, Undefined };

        public int Id { get; internal set; }
        public string Lemma { get; set; }
        public NounGender Gender { get; set; }

        public Noun(string text, NounGender gender) {
            Lemma = text;
            Gender = gender;
        }

        public Noun() {
            Lemma = "";
            Gender = NounGender.Undefined;
        }
    }
}