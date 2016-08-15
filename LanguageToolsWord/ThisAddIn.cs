using LanguageTools.Backend;
using LanguageTools.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using MSWord = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools;
using System.Reflection;
using Microsoft.Office.Tools.Ribbon;

namespace LanguageTools.Word
{
    public partial class ThisAddIn
    {
        private LemmaDatabase db;
        private LemmaRepository repo;
        private InstantLookup<MSWord.Document> instantLookup;
        private Dictionary<MSWord.Document, CustomTaskPane> taskPaneMap = new Dictionary<MSWord.Document, CustomTaskPane>();
        private bool taskPaneVisible = Properties.Settings.Default.LookupPaneVisible;
        private bool instantLookupEnabled = Properties.Settings.Default.InstantLookupEnabled;
        private int paneWidth = Properties.Settings.Default.LookupPaneWidth;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

            instantLookup = new InstantLookup<MSWord.Document>(new WordActiveTextStrategy(Application), 250, repo);
            instantLookup.OnLemmaFound += InstantLookup_OnLemmaFound;

            if (taskPaneVisible)
            {
                AddAllTaskPanes();
            }
            ((MSWord.ApplicationEvents4_Event)Application).NewDocument += ThisAddIn_NewDocument;
            Application.DocumentOpen += Application_DocumentOpen;
            Application.DocumentChange += Application_DocumentChange;

            ToggleInstantLookup(this, instantLookupEnabled, null);
        }

        private void InstantLookup_OnLemmaFound(object sender, List<Noun> found, MSWord.Document document)
        {
            CustomTaskPane ctp = null;
            taskPaneMap.TryGetValue(document, out ctp);
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
            List<MSWord.Document> removedDocuments = new List<MSWord.Document>();
            foreach (KeyValuePair<MSWord.Document, CustomTaskPane> entry in taskPaneMap)
            {
                if(entry.Value.Window == null)
                {
                    CustomTaskPanes.Remove(entry.Value);
                    removedDocuments.Add(entry.Key);
                }
            }
            foreach(MSWord.Document document in removedDocuments)
            {
                taskPaneMap.Remove(document);
            }
        }

        private void AddTaskPane(MSWord.Document doc)
        {
            CustomTaskPane taskPane = CustomTaskPanes.Add(new LookupPane(), "German Grammar", doc.ActiveWindow);
            taskPane.Control.Tag = taskPane;
            taskPane.Width = paneWidth;
            taskPane.Visible = true;
            taskPane.Control.SizeChanged += TaskPane_SizeChanged;
            taskPaneMap.Add(doc, taskPane);
        }

        private void PerformLookup(object sender, EventArgs e)
        {
            instantLookup.LookupActiveText(sender, e);
        }

        private void ToggleInstantLookup(object sender, bool value, RibbonControlEventArgs e)
        {
            instantLookup.Enabled = value;
            Globals.Ribbons.LanguageToolsRibbon.btnToggleInstantLookup.Checked = value;
            instantLookupEnabled = value;
        }

        private void ToggleLookupPane(object sender, bool visible, RibbonControlEventArgs e)
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

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            Properties.Settings.Default.LookupPaneVisible = taskPaneVisible;
            Properties.Settings.Default.InstantLookupEnabled = instantLookupEnabled;
            Properties.Settings.Default.LookupPaneWidth = paneWidth;
            Properties.Settings.Default.Save();
            db.CloseDatabase();
        }

        private void TaskPane_SizeChanged(object sender, EventArgs e)
        {
            // the tag was set in AddTaskPane. Ugly hack, I know.
            paneWidth = ((CustomTaskPane)((LookupPane)sender).Tag).Width;
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
            ribbon.OnLookupClicked += PerformLookup;
            ribbon.OnLookupPaneToggled += ToggleLookupPane;
            ribbon.OnInstantLookupToggled += ToggleInstantLookup;
            ribbon.OnInfoClicked += LanguageToolsRibbon_OnInfoClicked;

            ribbon.btnToggleInstantLookup.Checked = Properties.Settings.Default.InstantLookupEnabled;
            ribbon.btnToggleLookupPane.Checked = Properties.Settings.Default.LookupPaneVisible;
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