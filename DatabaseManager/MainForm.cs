using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DatabaseManager {
    public partial class MainForm : Form {
        private bool dbHasChanges = false;
        private Cache<Noun> memoryCache;
        private ProgressDialog progressDialog = new ProgressDialog();
        private LemmaDatabase db;
        private LemmaRepository repo;
        private bool updateDatabase = false;

        public MainForm() {
            InitializeComponent();
            foreach(Noun.NounGender g in Enum.GetValues(typeof(Noun.NounGender))) {
                this.gender.Items.Add(g);
                this.gender.ValueType = typeof(Noun.NounGender);
            }

            LoadDatabase(null);
        }

        private void bgwImportDictionary_DoWork(object sender, DoWorkEventArgs e) {
            db.OpenChangeSet();
            int count = 0, errAlreadyShownCount = 0;
            Importer importer = new DictCcImporter(openFileDialog.FileName);
            List<LemmaRepository.BulkItem> items = new List<LemmaRepository.BulkItem>();
            foreach(LemmaRepository.BulkItem l in importer.Items()) {
                items.Add(l);
                ++count;
                if(count % 10000 == 0) {
                    repo.AddBulk(items);
                    db.CommitChangeSet();
                    db.OpenChangeSet();
                    bgwImportDictionary.ReportProgress(importer.ProgressPercentage,
                        importer.ImportErrors.GetRange(errAlreadyShownCount, importer.ImportErrors.Count - errAlreadyShownCount));
                    errAlreadyShownCount = importer.ImportErrors.Count;
                    items.Clear();
                }
            }
            repo.AddBulk(items);
            db.CommitChangeSet();
            bgwImportDictionary.ReportProgress(100);

            importer.Close();
        }

        private void bgwImportDictionary_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progressDialog.progressBar.Value = e.ProgressPercentage;
            if(e.UserState != null) {
                List<ImportParsingException> errors = (List<ImportParsingException>)e.UserState;
                foreach(ImportParsingException ipe in errors) {
                    progressDialog.lbxMessages.Items.Add(ipe.Message);
                }
            }
        }

        private void bgwImportDictionary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            progressDialog.lbxMessages.Items.Add("Done.");
            progressDialog.btnClose.Enabled = true;
        }

        /// <summary>
        /// Asks for commit, if YES: commits, otherwise doesn't commit.
        /// </summary>
        /// <returns>
        /// DialogResult.OK if there was nothing to commit.
        /// DialogResult.Yes if changes were committed
        /// DialogResult.Cancel if the box was cancelled
        /// DialogResult.No if the user doesn't want to commit
        /// </returns>
        private DialogResult CommitChangesCancellable() {
            if(dbHasChanges) {
                DialogResult r = MessageBox.Show("There are unsaved changed. Do you want to commit them?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if(r == DialogResult.Yes) {
                    db.CommitChangeSet();
                    dbHasChanges = false;
                    db.OpenChangeSet();
                }
                return r;
            }
            return DialogResult.OK;
        }

        private void dgvData_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            if(e.RowIndex == dgvData.RowCount - 2 && e.RowIndex >= repo.Count) {
                return;
            }
            if(e.RowIndex == dgvData.RowCount - 1) {
                e.Value = null;
            } else {
                Noun l = memoryCache.RetrieveElement(e.RowIndex);
                switch(e.ColumnIndex) {
                    case 0: e.Value = l.Id; break;
                    case 1: e.Value = l.Lemma; break;
                    case 2: e.Value = l.Gender; break;
                    default: e.Value = null; break;
                }
            }
        }

        private void dgvData_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
            if(!updateDatabase) {
                return;
            }
            dbHasChanges = true;
            if(e.RowIndex >= repo.Count) {
                Noun l = new Noun();
                SetLemmaColumnValue(l, e.ColumnIndex, e.Value);
                repo.Add(l);
            } else {
                Noun l = memoryCache.RetrieveElement(e.RowIndex);
                SetLemmaColumnValue(l, e.ColumnIndex, e.Value);
                repo.Update(l);
            }
            dbHasChanges = true;
        }

        private void SetLemmaColumnValue(Noun l, int columnIndex, object value) {
            switch(columnIndex) {
                case 1: l.Lemma = Convert.ToString(value); break;
                case 2: l.Gender = (Noun.NounGender)value; break;
                default: throw new ArgumentException(string.Format("Cannot set column {0} to value {1}", columnIndex, value));
            }
        }

        private void dgvData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
            if(!updateDatabase) {
                return;
            }
            dbHasChanges = true;

            Noun l = memoryCache.RetrieveElement(e.RowIndex);
            repo.RemoveById(l.Id);
            memoryCache.ReloadAll();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void importDictionaryToolStripMenuItem1_Click(object sender, EventArgs e) {
            if(CommitChangesCancellable() == DialogResult.Cancel) {
                return;
            }
            db.RollBackChangeSet();
            if(openFileDialog.ShowDialog() == DialogResult.OK) {
                bgwImportDictionary.RunWorkerAsync();
                progressDialog.btnClose.Enabled = false;
                progressDialog.lbxMessages.Items.Clear();
                progressDialog.lbxMessages.Items.Add("Parsing file...");
                progressDialog.ShowDialog(this);
                RefreshGridView();
            }
            db.OpenChangeSet();
        }

        private void LoadDatabase(string fileName) {
            updateDatabase = false;
            if(db != null) {
                db.CloseDatabase();
            }
            if(fileName == null) {
                db = LemmaDatabase.CreateDefaultInstance();
            } else {
                db = new LemmaDatabase(fileName);
            }
            db.OpenChangeSet();
            repo = new LemmaRepository(db);
            RefreshGridView();
            updateDatabase = true;
        }

        private void RefreshGridView() {
            updateDatabase = false;
            dgvData.FirstDisplayedScrollingRowIndex = 0; // avoid ArrayIndexOutOfBounds due to showing lines after the last one (due to removed lines)
            memoryCache = new Cache<Noun>(repo, 100);
            dgvData.Rows.Clear();
            dgvData.RowCount = repo.Count + 1;
            updateDatabase = true;
        }

        private void OpenDatabaseFromDialog(FileDialog fileDialog) {
            if(CommitChangesCancellable() == DialogResult.Cancel) {
                return;
            }
            db.RollBackChangeSet();
            if(fileDialog.ShowDialog() == DialogResult.OK) {
                LoadDatabase(fileDialog.FileName);
            }
        }

        private void initializeNewDatabaseToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenDatabaseFromDialog(saveFileDialog);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = CommitChangesCancellable() == DialogResult.Cancel;
            db.CloseDatabase();
        }

        private void openDatabaseToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenDatabaseFromDialog(openFileDialog);
        }

        private void removeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e) {
            dbHasChanges = true;
            int nbRows = db.ExecuteNonQuery("delete from lemma where id not in (select min(id) as minid from lemma group by word, gender)", null);
            MessageBox.Show(nbRows + " duplicates removed");
            RefreshGridView();
        }
    }
}