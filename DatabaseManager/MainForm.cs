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

namespace DatabaseManager
{
    public partial class MainForm : Form
    {
        private SQLiteConnection conn = null;
        private SQLiteDataAdapter dataAdapter = null;
        private SQLiteCommandBuilder commandBuilder = null;
        private DataTable table;
        public MainForm()
        {
            InitializeComponent();
            SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder();
            bool newDb = !File.Exists("default.db");
            connBuilder.DataSource = "default.db";
            connBuilder.Version = 3;
            loadDatabase(connBuilder.ToString());
            if(newDb)
            {
                initializeDatabase();
            }
            loadTable("lemma");
        }

        private void loadDatabase(string connectionString)
        {
            closeDatabase();
            conn = new SQLiteConnection(connectionString);
            conn.Open();
        }

        private void closeDatabase()
        {
            if(conn != null)
            {
                commandBuilder = new SQLiteCommandBuilder(dataAdapter);
                DataTable changes = table.GetChanges();
                if (changes != null)
                {
                    dataAdapter.Update(changes);
                }
                conn.Close();
            }
        }

        private void loadTable(string tableName)
        {
            /*DataProvider provider = new DataProvider(conn, tableName);
            memoryCache = new Cache(provider, 100);
            dgvData.Columns.Clear();
            foreach(DataColumn column in provider.Columns)
            {
                dgvData.Columns.Add(column.ColumnName, column.ColumnName);
            }
            dgvData.RowCount = provider.RowCount;*/
            dataAdapter = new SQLiteDataAdapter("select * from lemma", conn);
            table = new DataTable("lemma");
            dataAdapter.Fill(table);
            dgvData.DataSource = table;
            dgvData.Columns["id"].Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void initializeNewDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder();
                connBuilder.DataSource = saveFileDialog.FileName;
                connBuilder.Version = 3;
                loadDatabase(connBuilder.ToString());
                initializeDatabase();
                loadTable("lemma");
            }
        }

        private void initializeDatabase()
        {
            SQLiteCommand command = new SQLiteCommand("create table lemma (id integer primary key not null, text varchar(100), gender int)", conn);
            command.ExecuteNonQuery();
        }

        private void openDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder();
                connBuilder.DataSource = openFileDialog.FileName;
                connBuilder.Version = 3;
                loadDatabase(connBuilder.ToString());
                loadTable("lemma");
            }
        }

        private void dgvData_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeDatabase();
        }
    }
}
