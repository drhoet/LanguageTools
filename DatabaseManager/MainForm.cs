using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseManager {
    public partial class MainForm : Form {
        private SQLiteConnection conn = null;
        private SQLiteDataAdapter dataAdapter = null;
        private SQLiteCommandBuilder commandBuilder = null;
        private DataTable table;
        private ProgressDialog progressDialog = new ProgressDialog();
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
        }

        private void closeDatabase() {
            conn.Close();
        }

        private void commitGuiChanges() {
            DataTable changes = table.GetChanges();
            if(changes != null) {
                dataAdapter.Update(changes);
            }
        }

        private void loadTable(string tableName) {
            /*DataProvider provider = new DataProvider(conn, tableName);
            memoryCache = new Cache(provider, 100);
            dgvData.Columns.Clear();
            foreach(DataColumn column in provider.Columns)
            {
                dgvData.Columns.Add(column.ColumnName, column.ColumnName);
            }
            dgvData.RowCount = provider.RowCount;*/
            dataAdapter = new SQLiteDataAdapter("select * from lemma", conn);
            commandBuilder = new SQLiteCommandBuilder(dataAdapter);
            table = new DataTable("lemma");
            dataAdapter.Fill(table);
            dgvData.DataSource = table;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
            //e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
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
            if(table.GetChanges() != null) {
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
                progressDialog.ShowDialog(this);
                loadTable("lemma");
            }
        }

        private void bgwImportDictionary_DoWork(object sender, DoWorkEventArgs e) {
            SQLiteCommand cmd = new SQLiteCommand("insert into lemma(text, gender) values(@text, @gender)", conn);
            cmd.Prepare();

            SQLiteTransaction tran = conn.BeginTransaction();

            int count = 0;
            Importer importer = new DictCcImporter(openFileDialog.FileName);
            foreach(Importer.Item l in importer.Items()) {
                cmd.Parameters.AddWithValue("@text", l.Word);
                cmd.Parameters.AddWithValue("@gender", l.Gender);
                cmd.ExecuteNonQuery();
                ++count;
                if(count % 100 == 0) {
                    tran.Commit();
                    tran = conn.BeginTransaction();
                }
                bgwImportDictionary.ReportProgress(importer.ProgressPercentage);
            }
            tran.Commit();

            importer.Close();
        }

        private void bgwImportDictionary_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progressDialog.progressBar.Value = e.ProgressPercentage;
        }

        private void bgwImportDictionary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            progressDialog.Close();
        }
    }
}
