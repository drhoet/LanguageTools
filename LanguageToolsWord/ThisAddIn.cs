using LanguageTools.Backend;
using LanguageTools.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using MSWord = Microsoft.Office.Interop.Word;

namespace LanguageTools.Word {
    public partial class ThisAddIn {
        private LookupPane lookupPane;
        private Microsoft.Office.Tools.CustomTaskPane germanGrammarTaskPane;
        private LemmaDatabase db;
        private LemmaRepository repo;
        private string lastLookup = null;
        private Timer lookupTimer;

        private void ThisAddIn_Startup(object sender, System.EventArgs e) {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

            lookupTimer = new Timer();
            lookupTimer.Interval = 250;
            lookupTimer.Tick += LookupWordUnderCursor;

            lookupPane = new LookupPane();
            germanGrammarTaskPane = this.CustomTaskPanes.Add(lookupPane, "German Grammar");
            germanGrammarTaskPane.Width = Properties.Settings.Default.LookupPaneWidth;

            ShowGermanGrammar(Properties.Settings.Default.GrammarPaneVisible);
            ToggleInstantLookup(Properties.Settings.Default.InstantLookupEnabled);
        }

        public void LookupWordUnderCursor(object sender, EventArgs args) {
            string searchFor = FindWordUnderCursor();
            if(searchFor.Length > 0) {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += bgw_DoWork;
                worker.RunWorkerAsync(searchFor);
            }
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e) {
            LookupValue(Convert.ToString(e.Argument));
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e) {
            Properties.Settings.Default.LookupPaneWidth = lookupPane.Width;
            Properties.Settings.Default.Save();
            db.CloseDatabase();
        }

        public void ShowGermanGrammar(bool visible) {
            germanGrammarTaskPane.Visible = visible;
            Properties.Settings.Default.GrammarPaneVisible = visible;
        }

        public void LookupValue(string value) {
            if(value != null && value != lastLookup) {
                lastLookup = value;
                List<Lemma> found = repo.FindAll(new GermanBaseLemmaSpecification(value));
                if(found.Count == 0) {
                    found.AddRange(repo.FindAll(new GermanCompositionEndLemmaSpecification(value)));
                }
                if(found.Count > 0) {
                    lookupPane.Invoke(new UpdateFoundItemsDelegate(UpdateFoundItems), found);
                }
            }
        }

        private delegate void UpdateFoundItemsDelegate(List<Lemma> list);

        private void UpdateFoundItems(List<Lemma> found) {
            lookupPane.Item = found.First();
            lookupPane.lbxResults.DataSource = found;
        }

        public void ToggleInstantLookup(bool value) {
            if(value) {
                lookupTimer.Start();
            } else {
                lookupTimer.Stop();
            }
            Globals.Ribbons.Ribbon1.btnToggleInstantLookup.Checked = value;
            Properties.Settings.Default.InstantLookupEnabled = value;
        }

        private string FindWordUnderCursor() {
            MSWord.Selection sel = Application.Selection;
            string searchFor = "";
            switch(sel.Type) {
                case MSWord.WdSelectionType.wdSelectionNormal:
                    if(sel.Text.Length == 1) {
                        searchFor = GetCompleteWordAt(sel);
                    } else {
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
        public static string GetCompleteWordAt(MSWord.Selection sel) {
            MSWord.Range word = sel.Document.Range(sel.Start, sel.End);
            word.StartOf(MSWord.WdUnits.wdWord, MSWord.WdMovementType.wdExtend);
            word.EndOf(MSWord.WdUnits.wdWord, MSWord.WdMovementType.wdExtend);
            return word.Text;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup() {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion VSTO generated code
    }
}