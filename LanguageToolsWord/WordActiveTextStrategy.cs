using LanguageTools.Common;
using System;
using MSWord = Microsoft.Office.Interop.Word;

namespace LanguageTools.Word
{
    public class WordActiveTextStrategy : ActiveTextStrategy<MSWord.Document>
    {
        public string FindActiveWord(MSWord.Document doc)
        {
            MSWord.Selection sel = doc.Application.Selection;
            string searchFor = "";
            switch (sel.Type)
            {
                case MSWord.WdSelectionType.wdSelectionNormal:
                    if (sel.Text.Length == 1)
                    {
                        searchFor = GetCompleteWordAt(sel);
                    }
                    else
                    {
                        searchFor = sel.Text;
                    }
                    break;
                case MSWord.WdSelectionType.wdSelectionIP:
                    searchFor = GetCompleteWordAt(sel);
                    break;
                default:
                    throw new InvalidOperationException("Unknown selection type: " + sel.Type.ToString());
            }

            return searchFor.Trim();
        }

        //TODO: [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCompleteWordAt(MSWord.Selection sel)
        {
            MSWord.Range word = sel.Document.Range(sel.Start, sel.End);
            word.StartOf(MSWord.WdUnits.wdWord, MSWord.WdMovementType.wdExtend);
            word.EndOf(MSWord.WdUnits.wdWord, MSWord.WdMovementType.wdExtend);
            return word.Text;
        }

    }
}
