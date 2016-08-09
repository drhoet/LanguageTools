using System;
using System.Collections.Generic;
using MSOutlook = Microsoft.Office.Interop.Outlook;
using LanguageTools.Backend;
using System.Windows.Forms;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Ribbon;
using System.ComponentModel;
using LanguageTools.Common;
using System.Reflection;

namespace LanguageTools.Outlook
{
    public partial class ThisAddIn
    {
        private LemmaDatabase db;
        private LemmaRepository repo;
        private string lastLookup = null;
        private Timer lookupTimer;
        private bool taskPaneVisible = Properties.Settings.Default.LookupPaneVisible;
        private bool instantLookup = Properties.Settings.Default.InstantLookupEnabled;
        private int paneWidth = Properties.Settings.Default.LookupPaneWidth;
        private OutlookActiveTextStrategy activeTextStrategy = new OutlookActiveTextStrategy();

        public Dictionary<MSOutlook.Inspector, InspectorWrapper> InspectorWrappers { get; private set; } = new Dictionary<MSOutlook.Inspector, InspectorWrapper>();
        private MSOutlook.Inspectors inspectors;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

            lookupTimer = new Timer();
            lookupTimer.Interval = 250;
            lookupTimer.Tick += LookupWordUnderCursor;

            inspectors = Application.Inspectors;
            inspectors.NewInspector += Inspectors_NewInspector;

            foreach (MSOutlook.Inspector inspector in inspectors)
            {
                Inspectors_NewInspector(inspector);
            }

            ToggleInstantLookup(this, instantLookup, null);
        }

        private void LanguageToolsRibbon_OnInfoClicked(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox(Assembly.GetExecutingAssembly());
            box.ShowDialog();
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
            if (value)
            {
                lookupTimer.Start();
            }
            else
            {
                lookupTimer.Stop();
            }
            //Globals.Ribbons.LanguageToolsRibbon.btnToggleInstantLookup.Checked = value;
            instantLookup = value;
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
                InspectorWrappers.Add(Inspector, new InspectorWrapper(Inspector));
            }
        }

        public void LookupWordUnderCursor(object sender, EventArgs eventArgs)
        {
            bool timerRunning = lookupTimer.Enabled;
            lookupTimer.Stop();

            string searchFor = activeTextStrategy.FindActiveWord(Application.ActiveInspector());
            if (searchFor.Length > 0)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += bgw_DoWork;
                worker.RunWorkerCompleted += bgw_WorkCompleted;
                SearchParams args = new SearchParams();
                args.SearchLemma = searchFor;
                args.TargetWindow = Application.ActiveInspector();
                worker.RunWorkerAsync(args);
            }

            if (timerRunning)
                lookupTimer.Start();
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            SearchParams args = (SearchParams)e.Argument;
            string value = args.SearchLemma;
            if (value != null && value != lastLookup)
            {
                lastLookup = value;
                List<Lemma> found = repo.FindAll(new GermanBaseLemmaSpecification(value));
                if (found.Count == 0)
                {
                    found.AddRange(repo.FindAll(new GermanCompositionEndLemmaSpecification(value)));
                }
                args.Found = found;
            }
            e.Result = args;
        }

        private void bgw_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchParams args = (SearchParams)e.Result;
            List<Lemma> found = args.Found;
            if (found != null && found.Count > 0)
            {
                InspectorWrapper wrapper = null;
                InspectorWrappers.TryGetValue(args.TargetWindow, out wrapper);
                if (wrapper != null)
                {
                    LookupPane lookupPane = (LookupPane)wrapper.CustomTaskPane.Control;
                    lookupPane.Invoke((Action)delegate
                    {
                        lookupPane.Item = found[0];
                        lookupPane.lbxResults.DataSource = found;
                    });
                }
            }
        }

        private struct SearchParams
        {
            public MSOutlook.Inspector TargetWindow { get; set; }
            public string SearchLemma { get; set; }
            public List<Lemma> Found { get; set; }
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

            ribbon.OnLookupClicked += LookupWordUnderCursor;
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
