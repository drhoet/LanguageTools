namespace LanguageTools.Backend
{
    public class Lemma
    {
        public int Id { get; internal set; }
        public string Word { get; set; }

        public Lemma(string word)
        {
            Word = word;
        }

        public Lemma()
        {
            Word = "";
        }
    }
}
