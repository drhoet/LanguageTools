namespace DatabaseManager
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initializeNewDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importDictionaryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDuplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.text = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gender = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.dsData = new System.Data.DataSet();
            this.bgwImportDictionary = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsData)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.dataToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(832, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initializeNewDatabaseToolStripMenuItem,
            this.openDatabaseToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.fileToolStripMenuItem.Text = "Database";
            // 
            // initializeNewDatabaseToolStripMenuItem
            // 
            this.initializeNewDatabaseToolStripMenuItem.Name = "initializeNewDatabaseToolStripMenuItem";
            this.initializeNewDatabaseToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.initializeNewDatabaseToolStripMenuItem.Text = "Initialize new database...";
            this.initializeNewDatabaseToolStripMenuItem.Click += new System.EventHandler(this.initializeNewDatabaseToolStripMenuItem_Click);
            // 
            // openDatabaseToolStripMenuItem
            // 
            this.openDatabaseToolStripMenuItem.Name = "openDatabaseToolStripMenuItem";
            this.openDatabaseToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.openDatabaseToolStripMenuItem.Text = "Open database...";
            this.openDatabaseToolStripMenuItem.Click += new System.EventHandler(this.openDatabaseToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importDictionaryToolStripMenuItem1,
            this.removeDuplicatesToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.dataToolStripMenuItem.Text = "Data";
            // 
            // importDictionaryToolStripMenuItem1
            // 
            this.importDictionaryToolStripMenuItem1.Name = "importDictionaryToolStripMenuItem1";
            this.importDictionaryToolStripMenuItem1.Size = new System.Drawing.Size(175, 22);
            this.importDictionaryToolStripMenuItem1.Text = "Import dictionary...";
            this.importDictionaryToolStripMenuItem1.Click += new System.EventHandler(this.importDictionaryToolStripMenuItem1_Click);
            // 
            // removeDuplicatesToolStripMenuItem
            // 
            this.removeDuplicatesToolStripMenuItem.Name = "removeDuplicatesToolStripMenuItem";
            this.removeDuplicatesToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.removeDuplicatesToolStripMenuItem.Text = "Remove duplicates";
            this.removeDuplicatesToolStripMenuItem.Click += new System.EventHandler(this.removeDuplicatesToolStripMenuItem_Click);
            // 
            // dgvData
            // 
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.text,
            this.gender});
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(0, 24);
            this.dgvData.MultiSelect = false;
            this.dgvData.Name = "dgvData";
            this.dgvData.ShowCellToolTips = false;
            this.dgvData.Size = new System.Drawing.Size(832, 503);
            this.dgvData.TabIndex = 1;
            this.dgvData.VirtualMode = true;
            this.dgvData.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvData_CellValueNeeded);
            this.dgvData.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvData_CellValuePushed);
            this.dgvData.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvData_RowsRemoved);
            // 
            // id
            // 
            this.id.HeaderText = "Identifier";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            // 
            // text
            // 
            this.text.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.text.HeaderText = "Text";
            this.text.Name = "text";
            // 
            // gender
            // 
            this.gender.HeaderText = "Gender";
            this.gender.Name = "gender";
            // 
            // dsData
            // 
            this.dsData.DataSetName = "NewDataSet";
            // 
            // bgwImportDictionary
            // 
            this.bgwImportDictionary.WorkerReportsProgress = true;
            this.bgwImportDictionary.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwImportDictionary_DoWork);
            this.bgwImportDictionary.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwImportDictionary_ProgressChanged);
            this.bgwImportDictionary.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwImportDictionary_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 527);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Language Tools Database Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initializeNewDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importDictionaryToolStripMenuItem1;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Data.DataSet dsData;
        private System.ComponentModel.BackgroundWorker bgwImportDictionary;
        private System.Windows.Forms.ToolStripMenuItem removeDuplicatesToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn text;
        private System.Windows.Forms.DataGridViewComboBoxColumn gender;
    }
}

