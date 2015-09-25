using LanguageTools.Backend;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private string lastLookup = null, pendingLookup = null;
        private WinHook keyboardHook;
        private System.Threading.Timer lookupTimerThread;

        private void ThisAddIn_Startup(object sender, System.EventArgs e) {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

            lookupTimerThread = new System.Threading.Timer(ExecutePendingLookup);
            keyboardHook = new WinHook(WinHook.HookType.WH_KEYBOARD, OnKeyPress);

            lookupPane = new LookupPane();
            germanGrammarTaskPane = this.CustomTaskPanes.Add(lookupPane, "German Grammar");

            ShowGermanGrammar(Properties.Settings.Default.GrammarPaneVisible);
            ToggleInstantLookup(Properties.Settings.Default.InstantLookupEnabled);
        }

        private void ExecutePendingLookup(object state) {
            LookupValue(pendingLookup);
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
                Globals.ThisAddIn.Application.WindowSelectionChange += OnWindowSelectionChange;
                using(Process proc = Process.GetCurrentProcess()) {
                    keyboardHook.InstallHook((IntPtr)0, proc.Threads[0].Id);
                }
                lookupTimerThread.Change(250, 250);
            } else {
                lookupTimerThread.Change(Timeout.Infinite, Timeout.Infinite);
                keyboardHook.ClearHook();
                Globals.ThisAddIn.Application.WindowSelectionChange -= OnWindowSelectionChange;
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

            searchFor = searchFor.Trim();
            if(searchFor.Length > 0) {
                pendingLookup = searchFor;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCompleteWordAt(MSWord.Selection sel) {
            MSWord.Range word = sel.Document.Range(sel.Start, sel.End);
            word.StartOf(MSWord.WdUnits.wdWord, MSWord.WdMovementType.wdExtend);
            word.EndOf(MSWord.WdUnits.wdWord, MSWord.WdMovementType.wdExtend);
            return word.Text;
        }

        private void OnKeyPress(int nCode, IntPtr wParam, IntPtr lParam) {
            const uint KF_UP = 0x80000000;
            const uint VK_BACK = 0x08, VK_0 = 0x30, VK_Z = 0x5A, VK_NUMPAD0 = 0x60, VK_NUMPAD9 = 0x69;
            const uint HC_ACTION = 0;
            // the WH_KEYBOARD hook fires every time the application (Here Word) calls GetMessage or PeekMessage and there
            // is a keyboard message to be processed. PeekMessage can be called with a flag PM_NOREMOVE, which doesn't remove
            // the message from the queue. That means if Word calls PeekMessage multiple times with this flag, we receive the
            // event here multiple times. If nCode == HC_ACTION, the message was removed from the queue and we know it won't
            // come again.
            if(nCode == HC_ACTION && ((uint)lParam & KF_UP) == KF_UP) {
                if((uint)wParam == VK_BACK ||
                    ((uint)wParam >= VK_0 && (uint)wParam <= VK_Z) ||
                    ((uint)wParam >= VK_NUMPAD0 && (uint)wParam <= VK_NUMPAD9)) {
                    try {
                        OnWindowSelectionChange(Globals.ThisAddIn.Application.Selection);
                    } catch(Exception e) {
                        MessageBox.Show(e.Message);
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