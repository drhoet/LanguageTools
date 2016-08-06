using LanguageTools.Common;

namespace LanguageTools.Word
{
    partial class ThisRibbonCollection : Microsoft.Office.Tools.Ribbon.RibbonReadOnlyCollection
    {
        internal LanguageToolsRibbon Ribbon1
        {
            get { return this.GetRibbon<LanguageToolsRibbon>(); }
        }
    }
}
