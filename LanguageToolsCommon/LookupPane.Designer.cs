namespace LanguageTools.Common
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
            this.lbxResults = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lbxResults
            // 
            this.lbxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxResults.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lbxResults.FormattingEnabled = true;
            this.lbxResults.Location = new System.Drawing.Point(0, 0);
            this.lbxResults.Name = "lbxResults";
            this.lbxResults.Size = new System.Drawing.Size(277, 407);
            this.lbxResults.TabIndex = 5;
            this.lbxResults.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbxResults_DrawItem);
            this.lbxResults.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lbxResults_MeasureItem);
            // 
            // LookupPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbxResults);
            this.Name = "LookupPane";
            this.Size = new System.Drawing.Size(277, 407);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.ListBox lbxResults;
    }
}
