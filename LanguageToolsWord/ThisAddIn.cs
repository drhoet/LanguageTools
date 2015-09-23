using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private void ThisAddIn_Startup(object sender, System.EventArgs e) {
            db = LemmaDatabase.CreateDefaultInstance();
            repo = new LemmaRepository(db);

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
            if(lookupPane.Item.Text != value) {
                List<Lemma> found = repo.FindAll(new GermanBaseLemmaSpecification(value));
                if(found.Count > 0) {
                    lookupPane.Item = found.First();
                    lookupPane.listBox1.DataSource = found;
                }
            }
        }

        public void ToggleInstantLookup(bool value) {
            if(value) {
                Globals.ThisAddIn.Application.WindowSelectionChange += OnWindowSelectionChange;
            } else {
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
                        searchFor = sel.Words[1].Text;
                    } else {
                        searchFor = sel.Text;
                    }
                    break;
                case MSWord.WdSelectionType.wdSelectionIP:
                    searchFor = sel.Words[1].Text;
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