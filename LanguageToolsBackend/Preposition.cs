using System;

namespace LanguageTools.Backend
{

    public class PrepositionCaseConvert
    {
        public static string ToString(Preposition.Case obj)
        {
            switch (obj)
            {
                case Preposition.Case.Akkusativ: return "+Akk.";
                case Preposition.Case.Dativ: return "+Dat.";
                case Preposition.Case.Genitiv: return "+Gen.";
                case Preposition.Case.Undefined: return "";
                default: throw new ArgumentException("Impossible value of Preposition.Case received: " + obj);
            }
        }

        public static Preposition.Case ToCase(object obj)
        {
            string str = Convert.ToString(obj).ToLower();
            switch (str)
            {
                case "+akk.": return Preposition.Case.Akkusativ;
                case "+dat.": return Preposition.Case.Dativ;
                case "+gen.": return Preposition.Case.Genitiv;
                case "": return Preposition.Case.Undefined;
                default: throw new ArgumentException("Invalid value for Preposition.Case: " + str);
            }
        }
    }

    public class Preposition: Lemma
    {
        public enum Case { Akkusativ, Dativ, Genitiv, Undefined };

        public Case PrepositionCase { get; set; }

        public Preposition(string word, Case prepCase): base(word)
        {
            PrepositionCase = prepCase;
        }

        public Preposition(): base()
        {
            PrepositionCase = Case.Undefined;
        }
    }
}
