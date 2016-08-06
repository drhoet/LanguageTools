using Microsoft.Office.Tools.Ribbon;

namespace LanguageTools.Common
{
    public interface LanguageToolsRibbonListener
    {
        void LookupWordUnderCursor();
        void ShowGermanGrammar(bool isChecked);
        void ToggleInstantLookup(bool isChecked);
    }

    public partial class LanguageToolsRibbon {

        private void LanguageToolsRibbon_Load(object sender, RibbonUIEventArgs e) {
        }

        private void btnLookup_Click(object sender, RibbonControlEventArgs e) {
            listener.LookupWordUnderCursor();
        }

        private void btnToggleLookupPane_Click(object sender, RibbonControlEventArgs e) {
            listener.ShowGermanGrammar(((RibbonToggleButton)sender).Checked);
        }

        private void btnToggleInstantLookup_Click(object sender, RibbonControlEventArgs e) {
            listener.ToggleInstantLookup(((RibbonToggleButton)sender).Checked);
        }
    }
}