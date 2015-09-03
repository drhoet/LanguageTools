using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseManager {
    public partial class MainForm : Form {
        private SQLiteConnection conn = null;
        private Cache memoryCache;
        private DataProvider provider;
        private ProgressDialog progressDialog = new ProgressDialog();
        private bool dbHasChanges = false;
        private SQLiteCommand insertCmd, updateCmd, deleteCmd;
        private SQLiteTransaction tran = null;
        private bool updateDatabase = false;

        public MainForm() {
            InitializeComponent();
            SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder();
            bool newDb = !File.Exists("default.db");
            connBuilder.DataSource = "default.db";
            connBuilder.Version = 3;
            loadDatabase(connBuilder.ToString());
            if(newDb) {
                initializeDatabase();
            }
            loadTable("lemma");
        }

        private void loadDatabase(string connectionString) {
            if(conn != null) {
                closeDatabase();
            }
            conn = new SQLiteConnection(connectionString);
            conn.Open();
            tran = conn.BeginTransaction();
        }

        private void closeDatabase() {
            tran.Rollback();
            conn.Close();
            updateDatabase = false;
        }

        private void commitGuiChanges() {
            tran.Commit();
            dbHasChanges = false;
            tran = conn.BeginTransaction();
        }

        private void loadTable(string tableName) {
            updateDatabase = false;
            provider = new DataProvider(conn, tableName);
            memoryCache = new Cache(provider, 100);
            dgvData.VirtualMode = true;
            dgvData.Columns.Clear();
            foreach(DataColumn column in provider.Columns)
            {
                dgvData.Columns.Add(column.ColumnName, column.ColumnName);
            }
            dgvData.RowCount = provider.RowCount + 1;

            if(dgvData.Columns.Contains("text")) {
                dgvData.Columns["text"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            insertCmd = new SQLiteCommand("insert into lemma(text, gender) values(@text, @gender)", conn);
            insertCmd.Prepare();
            updateCmd = new SQLiteCommand("update lemma set text=@text, gender=@gender where id=@id", conn);
            updateCmd.Prepare();
            deleteCmd = new SQLiteCommand("delete from lemma where id=@id", conn);
            deleteCmd.Prepare();
            updateDatabase = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void initializeNewDatabaseToolStripMenuItem_Click(object sender, EventArgs e) {
            if(commitChangesCancellable() == DialogResult.Cancel) {
                return;
            }
            if(saveFileDialog.ShowDialog() == DialogResult.OK) {
                SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder();
                connBuilder.DataSource = saveFileDialog.FileName;
                connBuilder.Version = 3;
                loadDatabase(connBuilder.ToString());
                initializeDatabase();
                loadTable("lemma");
            }
        }

        private void initializeDatabase() {
            SQLiteCommand command = new SQLiteCommand("create table lemma (id integer primary key not null, text varchar(100), gender varchar(2))", conn);
            command.ExecuteNonQuery();
            tran.Commit();
            tran = conn.BeginTransaction();
        }

        private void openDatabaseToolStripMenuItem_Click(object sender, EventArgs e) {
            if(openFileDialog.ShowDialog() == DialogResult.OK) {
                SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder();
                connBuilder.DataSource = openFileDialog.FileName;
                connBuilder.Version = 3;
                loadDatabase(connBuilder.ToString());
                loadTable("lemma");
            }
        }

        private void dgvData_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            if(e.RowIndex == dgvData.RowCount - 2 && e.RowIndex >= provider.RowCount) {
                return;
            }
            if(e.RowIndex == dgvData.RowCount - 1) {
                e.Value = "";
            } else {
                e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
            }
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
        private DialogResult commitChangesCancellable() {
            if(dbHasChanges) {
                DialogResult r = MessageBox.Show("There are unsaved changed. Do you want to commit them?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if(r == DialogResult.Yes) {
                    commitGuiChanges();
                }
                return r;
            }
            return DialogResult.OK;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = commitChangesCancellable() == DialogResult.Cancel;
            closeDatabase();
        }

        private void importDictionaryToolStripMenuItem1_Click(object sender, EventArgs e) {
            if(commitChangesCancellable() == DialogResult.Cancel) {
                return;
            }
            if(openFileDialog.ShowDialog() == DialogResult.OK) {
                bgwImportDictionary.RunWorkerAsync();
                progressDialog.btnClose.Enabled = false;
                progressDialog.lbxMessages.Items.Clear();
                progressDialog.lbxMessages.Items.Add("Parsing file...");
                progressDialog.ShowDialog(this);
                loadTable("lemma");
            }
        }

        private void bgwImportDictionary_DoWork(object sender, DoWorkEventArgs e) {
            SQLiteTransaction importTran = conn.BeginTransaction();

            int count = 0, errAlreadyShownCount = 0;
            Importer importer = new DictCcImporter(openFileDialog.FileName);
            foreach(Importer.Item l in importer.Items()) {
                insertCmd.Parameters.AddWithValue("@text", l.Word);
                insertCmd.Parameters.AddWithValue("@gender", l.Gender);
                insertCmd.ExecuteNonQuery();
                ++count;
                if(count % 10000 == 0) {
                    importTran.Commit();
                    importTran = conn.BeginTransaction();
                    bgwImportDictionary.ReportProgress(importer.ProgressPercentage,
                        importer.ImportErrors.GetRange(errAlreadyShownCount, importer.ImportErrors.Count - errAlreadyShownCount));
                    errAlreadyShownCount = importer.ImportErrors.Count;
                }
            }
            importTran.Commit();
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

        private void dgvData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
            if(!updateDatabase) {
                return;
            }
            dbHasChanges = true;
            deleteCmd.Parameters.AddWithValue("id", dgvData["id", e.RowIndex].Value);
            deleteCmd.ExecuteNonQuery();
            memoryCache.ReloadAll();
        }

        private void bgwImportDictionary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            progressDialog.lbxMessages.Items.Add("Done.");
            progressDialog.btnClose.Enabled = true;
        }

        private void dgvData_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
            if(!updateDatabase) {
                return;
            }
            dbHasChanges = true;
            if(e.RowIndex >= provider.RowCount) {
                for(int c = 0; c < dgvData.Columns.Count; ++c) {
                    if(c == e.ColumnIndex) {
                        insertCmd.Parameters.AddWithValue(dgvData.Columns[c].Name, e.Value);
                    } else {
                        insertCmd.Parameters.AddWithValue(dgvData.Columns[c].Name, string.Empty);
                        dgvData.InvalidateRow(e.RowIndex);
                    }
                }
                insertCmd.ExecuteNonQuery();
            } else {
                for(int c = 0; c < dgvData.Columns.Count; ++c) {
                    if(c == e.ColumnIndex) {
                        updateCmd.Parameters.AddWithValue(dgvData.Columns[c].Name, e.Value);
                    } else {
                        updateCmd.Parameters.AddWithValue(dgvData.Columns[c].Name, memoryCache.RetrieveElement(e.RowIndex, c));
                    }
                }
                updateCmd.ExecuteNonQuery();
            }

            memoryCache.ReloadAll();
        }
    }
}
