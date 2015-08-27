using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;

namespace LanguageTools.Word
{
    public partial class LanguageToolsRibbon
    {
        private void LanguageToolsRibbon_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void btnLookup_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.LookupValue(Globals.ThisAddIn.Application.Selection.Text);
        }

        private void btnToggleLookupPane_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.ShowGermanGrammar(((RibbonToggleButton)sender).Checked);
        }

        private void btnToggleInstantLookup_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.ToggleInstantLookup(((RibbonToggleButton)sender).Checked);
        }

    }
}
