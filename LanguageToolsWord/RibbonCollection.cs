using LanguageTools.Common;

namespace LanguageTools.Word
{
    partial class ThisRibbonCollection : Microsoft.Office.Tools.Ribbon.RibbonReadOnlyCollection
    {
        internal LanguageToolsRibbon LanguageToolsRibbon
        {
            get { return this.GetRibbon<LanguageToolsRibbon>(); }
        }
    }
}
