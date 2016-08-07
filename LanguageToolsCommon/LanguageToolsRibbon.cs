using Microsoft.Office.Tools.Ribbon;
using System;

namespace LanguageTools.Common
{
    public delegate void ToggleEventHandler(object sender, bool value);

    public partial class LanguageToolsRibbon
    {

        public event EventHandler OnLookupClicked;
        public event ToggleEventHandler OnLookupPaneToggled;
        public event ToggleEventHandler OnInstantLookupToggled;
        public event EventHandler OnInfoClicked;

        private void btnLookup_Click(object sender, RibbonControlEventArgs e)
        {
            OnLookupClicked?.Invoke(sender, EventArgs.Empty);
        }

        private void btnToggleLookupPane_Click(object sender, RibbonControlEventArgs e)
        {
            OnLookupPaneToggled?.Invoke(sender, ((RibbonToggleButton)sender).Checked);
        }

        private void btnToggleInstantLookup_Click(object sender, RibbonControlEventArgs e)
        {
            OnInstantLookupToggled?.Invoke(sender, ((RibbonToggleButton)sender).Checked);
        }

        private void btnInfo_Click(object sender, RibbonControlEventArgs e)
        {
            OnInfoClicked?.Invoke(sender, EventArgs.Empty);
        }
    }
}