using LanguageTools.Backend;
using LanguageTools.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using MSWord = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools;

namespace LanguageTools.Word
{
    public partial class ThisAddIn
    {
        private LookupPane lookupPane;
        private CustomTaskPane germanGrammarTaskPane;
        private LemmaDatabase db;
        private LemmaRepository repo;
        private string lastLookup = null;
        private Timer lookupTimer;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

            lookupTimer = new Timer();
            lookupTimer.Interval = 250;
            lookupTimer.Tick += LookupWordUnderCursor;

            lookupPane = new LookupPane();
            germanGrammarTaskPane = CustomTaskPanes.Add(lookupPane, "German Grammar");
            germanGrammarTaskPane.Width = Properties.Settings.Default.LookupPaneWidth;

            Globals.Ribbons.LanguageToolsRibbon.OnLookupClicked += LookupWordUnderCursor;
            Globals.Ribbons.LanguageToolsRibbon.OnLookupPaneToggled += ToggleLookupPane;
            ToggleLookupPane(this, Properties.Settings.Default.LookupPaneVisible);
            Globals.Ribbons.LanguageToolsRibbon.OnInstantLookupToggled += ToggleInstantLookup;
            ToggleInstantLookup(this, Properties.Settings.Default.InstantLookupEnabled);
        }

        private void ToggleInstantLookup(object sender, bool value)
        {
            if (value)
            {
                lookupTimer.Start();
            }
            else
            {
                lookupTimer.Stop();
            }
            Globals.Ribbons.LanguageToolsRibbon.btnToggleInstantLookup.Checked = value;
            Properties.Settings.Default.InstantLookupEnabled = value;
        }

        private void ToggleLookupPane(object sender, bool visible)
        {
            germanGrammarTaskPane.Visible = visible;
        }

        public void LookupWordUnderCursor(object sender, EventArgs args)
        {
            string searchFor = FindWordUnderCursor();
            if (searchFor.Length > 0)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += bgw_DoWork;
                worker.RunWorkerCompleted += bgw_WorkCompleted;
                worker.RunWorkerAsync(searchFor);
            }
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            string value = Convert.ToString(e.Argument);
            if (value != null && value != lastLookup)
            {
                lastLookup = value;
                List<Lemma> found = repo.FindAll(new GermanBaseLemmaSpecification(value));
                if (found.Count == 0)
                {
                    found.AddRange(repo.FindAll(new GermanCompositionEndLemmaSpecification(value)));
                }
                e.Result = found;
            }
        }

        private void bgw_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<Lemma> found = (List<Lemma>)e.Result;
            if (found != null && found.Count > 0)
            {
                lookupPane.Invoke( (Action)delegate {
                    lookupPane.Item = found.First();
                    lookupPane.lbxResults.DataSource = found;
                });
            }
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            Properties.Settings.Default.LookupPaneWidth = lookupPane.Width;
            Properties.Settings.Default.LookupPaneVisible = lookupPane.Visible;
            Properties.Settings.Default.Save();
            db.CloseDatabase();
        }

        private string FindWordUnderCursor()
        {
            MSWord.Selection sel = Application.Selection;
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
                    MessageBox.Show("Unknown selection type: " + sel.Type.ToString());
                    break;
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

        protected override Microsoft.Office.Tools.Ribbon.IRibbonExtension[] CreateRibbonObjects()
        {
            LanguageToolsRibbon ribbon = new LanguageToolsRibbon(Globals.Factory.GetRibbonFactory());
            ribbon.btnToggleInstantLookup.Checked = Properties.Settings.Default.InstantLookupEnabled;
            ribbon.btnToggleLookupPane.Checked = Properties.Settings.Default.LookupPaneVisible;
            return new Microsoft.Office.Tools.Ribbon.IRibbonExtension[] { ribbon };
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

        #endregion VSTO generated code
    }
}