namespace LanguageTools.Word
{
    partial class LookupPane
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chxInstantLookup = new System.Windows.Forms.CheckBox();
            this.lbxResults = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // chxInstantLookup
            // 
            this.chxInstantLookup.AutoSize = true;
            this.chxInstantLookup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chxInstantLookup.Location = new System.Drawing.Point(0, 390);
            this.chxInstantLookup.Name = "chxInstantLookup";
            this.chxInstantLookup.Size = new System.Drawing.Size(277, 17);
            this.chxInstantLookup.TabIndex = 2;
            this.chxInstantLookup.Text = "Instant lookup";
            this.chxInstantLookup.UseVisualStyleBackColor = true;
            this.chxInstantLookup.Click += new System.EventHandler(this.chxInstantLookup_Click);
            // 
            // lbxResults
            // 
            this.lbxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxResults.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lbxResults.FormattingEnabled = true;
            this.lbxResults.Location = new System.Drawing.Point(0, 0);
            this.lbxResults.Name = "lbxResults";
            this.lbxResults.Size = new System.Drawing.Size(277, 390);
            this.lbxResults.TabIndex = 5;
            this.lbxResults.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbxResults_DrawItem);
            this.lbxResults.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lbxResults_MeasureItem);
            // 
            // LookupPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbxResults);
            this.Controls.Add(this.chxInstantLookup);
            this.Name = "LookupPane";
            this.Size = new System.Drawing.Size(277, 407);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.CheckBox chxInstantLookup;
        public System.Windows.Forms.ListBox lbxResults;
    }
}
