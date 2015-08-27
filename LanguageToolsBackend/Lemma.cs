using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend
{
    public class Lemma
    {
        public enum WordGender { Mannlich, Weiblich, Neutral };

        public string Text { get; set; }
        public WordGender Gender { get; set; }

        public Lemma(string text, WordGender gender)
        {
            Text = text;
            Gender = gender;
        }
    }
}
