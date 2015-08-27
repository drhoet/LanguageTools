using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend
{
    public class Backend
    {
        private static Backend instance = null;

        public static Backend getInstance()
        {
            if(instance == null)
            {
                instance = new Backend();
            }

            return instance;
        }

        public Lemma FindLemma(string searchFor)
        {
            return new Lemma(searchFor, Lemma.WordGender.Mannlich);
        }

        private Backend()
        {
            Initialize();
        }

        private void Initialize()
        {

        }
    }
}
