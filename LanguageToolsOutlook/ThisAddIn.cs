using System;
using System.Collections.Generic;
using MSOutlook = Microsoft.Office.Interop.Outlook;
using LanguageTools.Backend;
using System.Windows.Forms;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Ribbon;
using LanguageTools.Common;
using System.Reflection;

namespace LanguageTools.Outlook
{
    public partial class ThisAddIn
    {
        private LemmaDatabase db;
        private LemmaRepository repo;
        private InstantLookup<MSOutlook.Inspector> instantLookup;
        private bool taskPaneVisible = Properties.Settings.Default.LookupPaneVisible;
        private bool instantLookupEnabled = Properties.Settings.Default.InstantLookupEnabled;
        private int paneWidth = Properties.Settings.Default.LookupPaneWidth;

        public Dictionary<MSOutlook.Inspector, InspectorWrapper> InspectorWrappers { get; private set; } = new Dictionary<MSOutlook.Inspector, InspectorWrapper>();
        private MSOutlook.Inspectors inspectors;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

            instantLookup = new InstantLookup<MSOutlook.Inspector>( new OutlookActiveTextStrategy(Application), 250, repo );

            inspectors = Application.Inspectors;
            inspectors.NewInspector += Inspectors_NewInspector;

            foreach (MSOutlook.Inspector inspector in inspectors)
            {
                Inspectors_NewInspector(inspector);
            }

            ToggleInstantLookup(this, instantLookupEnabled, null);
        }
        
        private void LanguageToolsRibbon_OnInfoClicked(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox(Assembly.GetExecutingAssembly());
            box.ShowDialog();
        }

        private void PerformLookup(object sender, EventArgs e)
        {
            instantLookup.LookupActiveText(sender, e);
        }

        private void ToggleLookupPane(object sender, bool value, RibbonControlEventArgs e)
        {
            MSOutlook.Inspector inspector = (MSOutlook.Inspector)e.Control.Context;
            InspectorWrapper inspectorWrapper = Globals.ThisAddIn.InspectorWrappers[inspector];
            CustomTaskPane taskPane = inspectorWrapper.CustomTaskPane;
            if (taskPane != null)
            {
                taskPane.Visible = ((RibbonToggleButton)sender).Checked;
            }
        }

        private void ToggleInstantLookup(object sender, bool value, RibbonControlEventArgs e)
        {
            instantLookup.Enabled = value;
            //Globals.Ribbons.LanguageToolsRibbon.btnToggleInstantLookup.Checked = value;
            instantLookupEnabled = value;
        }


        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
            inspectors.NewInspector -= Inspectors_NewInspector;
            inspectors = null;
            InspectorWrappers = null;
        }

        private void Inspectors_NewInspector(MSOutlook.Inspector Inspector)
        {
            if (Inspector.CurrentItem is MSOutlook.MailItem)
            {
                InspectorWrappers.Add(Inspector, new InspectorWrapper(Inspector, instantLookup));
            }
        }

        protected override IRibbonExtension[] CreateRibbonObjects()
        {
            LanguageToolsRibbon.GlobalsFactory = Globals.Factory;
            LanguageToolsRibbon.OnRibbonCreated += LanguageToolsRibbon_OnRibbonCreated;
            return new IRibbonExtension[] { new LanguageToolsRibbon() };
        }

        private void LanguageToolsRibbon_OnRibbonCreated(object sender, EventArgs e)
        {
            LanguageToolsRibbon ribbon = (LanguageToolsRibbon)sender;
            ribbon.btnToggleInstantLookup.Checked = Properties.Settings.Default.InstantLookupEnabled;
            ribbon.btnToggleLookupPane.Checked = Properties.Settings.Default.LookupPaneVisible;

            ribbon.OnLookupClicked += PerformLookup;
            ribbon.OnLookupPaneToggled += ToggleLookupPane;
            ribbon.OnInstantLookupToggled += ToggleInstantLookup;
            ribbon.OnInfoClicked += LanguageToolsRibbon_OnInfoClicked;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
