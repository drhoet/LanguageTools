using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        private BackgroundWorker lookupWorker = null;
        private WinHook keyboardHook;

        private void ThisAddIn_Startup(object sender, System.EventArgs e) {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

            keyboardHook = new WinHook(WinHook.HookType.WH_KEYBOARD, OnKeyPress);

            lookupPane = new LookupPane();
            germanGrammarTaskPane = this.CustomTaskPanes.Add(lookupPane, "German Grammar");

            ShowGermanGrammar(Properties.Settings.Default.GrammarPaneVisible);
            ToggleInstantLookup(Properties.Settings.Default.InstantLookupEnabled);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e) {
            Properties.Settings.Default.Save();
            db.CloseDatabase();
        }

        public void ShowGermanGrammar(bool visible) {
            germanGrammarTaskPane.Visible = visible;
            Properties.Settings.Default.GrammarPaneVisible = visible;
        }

        public void LookupValue(string value) {
            if(value != lastLookup) {
                lastLookup = value;
                if(lookupWorker == null) { //this way, no two lookups run together
                    //below line fixes a bug in VSTO with BackgroundWorker
                    SynchronizationContext.SetSynchronizationContext(lookupPane.SyncCtx);
                    lookupWorker = new BackgroundWorker();
                    lookupWorker.DoWork += OnDoLookupValue;
                    lookupWorker.RunWorkerCompleted += OnDoLookupCompleted;
                    lookupWorker.RunWorkerAsync(value);
                } else {
                    Debug.WriteLine(string.Format("Ignoring search for {0} because another search is running", value));
                }
            }
        }

        private void OnDoLookupValue(object sender, DoWorkEventArgs e) {
            string value = Convert.ToString(e.Argument);
            List<Lemma> found = repo.FindAll(new GermanBaseLemmaSpecification(value));
            if(found.Count == 0) {
                found.AddRange(repo.FindAll(new GermanCompositionEndLemmaSpecification(value)));
            }
            e.Result = found;
        }

        private void OnDoLookupCompleted(object sender, RunWorkerCompletedEventArgs e) {
            List<Lemma> found = (List<Lemma>)e.Result;
            if(found.Count > 0) {
                lookupPane.Item = found.First();
                lookupPane.lbxResults.DataSource = found;
            }
            lookupWorker = null;
        }

        public void ToggleInstantLookup(bool value) {
            if(value) {
                //Globals.ThisAddIn.Application.WindowSelectionChange += OnWindowSelectionChange;
                using(Process proc = Process.GetCurrentProcess())
                using(ProcessModule mod = proc.MainModule) {
                    keyboardHook.InstallHook((IntPtr)0, proc.Threads[0].Id);
                }
            } else {
                keyboardHook.ClearHook();
                //Globals.ThisAddIn.Application.WindowSelectionChange -= OnWindowSelectionChange;
            }
            lookupPane.chxInstantLookup.Checked = value;
            Globals.Ribbons.Ribbon1.btnToggleInstantLookup.Checked = value;
            Properties.Settings.Default.InstantLookupEnabled = value;
        }

        private void OnWindowSelectionChange(MSWord.Selection sel) {
            string searchFor = "";
            switch(sel.Type) {
                case MSWord.WdSelectionType.wdSelectionNormal:
                    if(sel.Text.Length == 1) {
                        searchFor = sel.Words[1].Text;
                    } else {
                        searchFor = sel.Text;
                    }
                    break;
                case MSWord.WdSelectionType.wdSelectionIP:
                    searchFor = sel.Words[1].Text;
                    if(searchFor.Trim().Length == 0) {
                        MSWord.Range word = sel.Document.Range(sel.Start, sel.End);
                        word.StartOf(MSWord.WdUnits.wdWord, MSWord.WdMovementType.wdExtend);
                        searchFor = word.Text;
                    }
                    break;
                default:
                    MessageBox.Show("Unknown selection type: " + sel.Type.ToString());
                    break;
            }

            searchFor = searchFor.Trim();
            if(searchFor.Length > 0) {
                LookupValue(searchFor);
            }
        }

        private void OnKeyPress(int nCode, IntPtr wParam, IntPtr lParam) {
            const uint KF_UP = 0x80000000;
            const uint VK_BACK = 0x08, VK_0 = 0x30, VK_Z = 0x5A, VK_NUMPAD0 = 0x60, VK_NUMPAD9 = 0x69;
            if(((uint)lParam & KF_UP) == KF_UP) {
                if((uint)wParam == VK_BACK ||
                    ((uint)wParam >= VK_0 && (uint)wParam <= VK_Z) ||
                    ((uint)wParam >= VK_NUMPAD0 && (uint)wParam <= VK_NUMPAD9)) {
                    try {
                        OnWindowSelectionChange(Globals.ThisAddIn.Application.Selection);
                    } catch(Exception e) {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
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