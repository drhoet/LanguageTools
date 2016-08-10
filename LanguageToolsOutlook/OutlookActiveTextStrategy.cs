using LanguageTools.Common;
using MSOutlook = Microsoft.Office.Interop.Outlook;
using MSWord = Microsoft.Office.Interop.Word;
using LanguageTools.Word;
using System;

namespace LanguageTools.Outlook
{
    public class OutlookActiveTextStrategy : ActiveTextStrategy<MSOutlook.Inspector>
    {
        private WordActiveTextStrategy wrappedWordStrategy = new WordActiveTextStrategy(null);

        public MSOutlook.Application Application { get; private set; }
        public OutlookActiveTextStrategy(MSOutlook.Application application)
        {
            Application = application;
        }

        public string FindActiveWord()
        {
            return FindActiveWord(FindActiveDocument());
        }

        public string FindActiveWord(MSOutlook.Inspector inspector)
        {
            MSWord.Document document = (MSWord.Document)inspector.WordEditor;
            return wrappedWordStrategy.FindActiveWord(document);
        }

        public MSOutlook.Inspector FindActiveDocument()
        {
            return Application.ActiveInspector();
        }
    }
}
