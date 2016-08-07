using LanguageTools.Backend;
using LanguageTools.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using MSWord = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools;
using System.Reflection;

namespace LanguageTools.Word
{
    public partial class ThisAddIn
    {
        private LemmaDatabase db;
        private LemmaRepository repo;
        private string lastLookup = null;
        private Timer lookupTimer;
        private Dictionary<MSWord.Window, CustomTaskPane> taskPaneMap = new Dictionary<MSWord.Window, CustomTaskPane>();
        private bool taskPaneVisible = Properties.Settings.Default.LookupPaneVisible;
        private bool instantLookup = Properties.Settings.Default.InstantLookupEnabled;
        private int paneWidth = Properties.Settings.Default.LookupPaneWidth;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

            lookupTimer = new Timer();
            lookupTimer.Interval = 250;
            lookupTimer.Tick += LookupWordUnderCursor;

            if (taskPaneVisible)
            {
                AddAllTaskPanes();
            }
            ((MSWord.ApplicationEvents4_Event)Application).NewDocument += ThisAddIn_NewDocument;
            Application.DocumentOpen += Application_DocumentOpen;
            Application.DocumentChange += Application_DocumentChange;

            Globals.Ribbons.LanguageToolsRibbon.OnLookupClicked += LookupWordUnderCursor;
            Globals.Ribbons.LanguageToolsRibbon.OnLookupPaneToggled += ToggleLookupPane;
            Globals.Ribbons.LanguageToolsRibbon.OnInstantLookupToggled += ToggleInstantLookup;
            Globals.Ribbons.LanguageToolsRibbon.OnInfoClicked += LanguageToolsRibbon_OnInfoClicked;
            ToggleInstantLookup(this, instantLookup);
        }

        private void LanguageToolsRibbon_OnInfoClicked(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox(Assembly.GetExecutingAssembly());
            box.ShowDialog();
        }

        private void Application_DocumentChange()
        {
            RemoveOrphanedTaskPanes();
        }

        private void Application_DocumentOpen(MSWord.Document Doc)
        {
            RemoveOrphanedTaskPanes();
            if (taskPaneVisible && Application.ShowWindowsInTaskbar)
            {
                AddTaskPane(Doc);
            }
        }

        private void ThisAddIn_NewDocument(MSWord.Document Doc)
        {
            if (taskPaneVisible && Application.ShowWindowsInTaskbar)
            {
                AddTaskPane(Doc);
            }
        }

        private void AddAllTaskPanes()
        {
            if (Globals.ThisAddIn.Application.Documents.Count > 0)
            {
                if (Application.ShowWindowsInTaskbar == true)
                {
                    foreach (MSWord.Document doc in Application.Documents)
                    {
                        AddTaskPane(doc);
                    }
                }
                else
                {
                    AddTaskPane(Application.ActiveDocument);
                }
            }
        }

        private void RemoveAllTaskPanes()
        {
            foreach (CustomTaskPane pane in taskPaneMap.Values)
            {
                CustomTaskPanes.Remove(pane);
            }
            taskPaneMap.Clear();
        }

        private void RemoveOrphanedTaskPanes()
        {
            List<MSWord.Window> removedWindows = new List<MSWord.Window>();
            foreach (KeyValuePair<MSWord.Window, CustomTaskPane> entry in taskPaneMap)
            {
                if(entry.Value.Window == null)
                {
                    CustomTaskPanes.Remove(entry.Value);
                    removedWindows.Add(entry.Key);
                }
            }
            foreach(MSWord.Window window in removedWindows)
            {
                taskPaneMap.Remove(window);
            }
        }

        private void AddTaskPane(MSWord.Document doc)
        {
            CustomTaskPane taskPane = CustomTaskPanes.Add(new LookupPane(), "German Grammar", doc.ActiveWindow);
            taskPane.Control.Tag = taskPane;
            taskPane.Width = paneWidth;
            taskPane.Visible = true;
            taskPane.Control.SizeChanged += TaskPane_SizeChanged;
            taskPaneMap.Add(taskPane.Window, taskPane);

            lastLookup = null; //setting this to null will make sure a new search is done, otherwise the newly opened panels stay empty
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
            instantLookup = value;
        }

        private void ToggleLookupPane(object sender, bool visible)
        {
            taskPaneVisible = visible;
            if (visible)
            {
                AddAllTaskPanes();
            }
            else
            {
                RemoveAllTaskPanes();
            }
        }

        public void LookupWordUnderCursor(object sender, EventArgs eventArgs)
        {
            bool timerRunning = lookupTimer.Enabled;
            lookupTimer.Stop();

            string searchFor = FindWordUnderCursor(Application.ActiveWindow);
            if (searchFor.Length > 0)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += bgw_DoWork;
                worker.RunWorkerCompleted += bgw_WorkCompleted;
                SearchParams args = new SearchParams();
                args.SearchLemma = searchFor;
                args.TargetWindow = Application.ActiveWindow;
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
                CustomTaskPane ctp = null;
                taskPaneMap.TryGetValue(args.TargetWindow, out ctp);
                if (ctp != null)
                {
                    LookupPane lookupPane = (LookupPane)ctp.Control;
                    lookupPane.Invoke((Action)delegate
                    {
                        lookupPane.Item = found[0];
                        lookupPane.lbxResults.DataSource = found;
                    });
                }
            }
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            Properties.Settings.Default.LookupPaneVisible = taskPaneVisible;
            Properties.Settings.Default.InstantLookupEnabled = instantLookup;
            Properties.Settings.Default.LookupPaneWidth = paneWidth;
            Properties.Settings.Default.Save();
            db.CloseDatabase();
        }

        private string FindWordUnderCursor(MSWord.Window targetWindow)
        {
            MSWord.Selection sel = targetWindow.Selection;
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

        private void TaskPane_SizeChanged(object sender, EventArgs e)
        {
            // the tag was set in AddTaskPane. Ugly hack, I know.
            paneWidth = ((CustomTaskPane)((LookupPane)sender).Tag).Width;
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

        private struct SearchParams
        {
            public MSWord.Window TargetWindow { get; set; }
            public string SearchLemma { get; set; }
            public List<Lemma> Found { get; set; }
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