using LanguageTools.Common;
using MSOutlook = Microsoft.Office.Interop.Outlook;
using MSWord = Microsoft.Office.Interop.Word;
using LanguageTools.Word;

namespace LanguageTools.Outlook
{
    public class OutlookActiveTextStrategy : ActiveTextStrategy<MSOutlook.Inspector>
    {
        private WordActiveTextStrategy wrappedWordStrategy = new WordActiveTextStrategy();

        public string FindActiveWord(MSOutlook.Inspector inspector)
        {
            MSWord.Document document = (MSWord.Document)inspector.WordEditor;
            return wrappedWordStrategy.FindActiveWord(document);
        }
    }
}
