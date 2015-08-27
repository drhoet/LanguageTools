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
            this.btnLookup = new System.Windows.Forms.Button();
            this.txtLemma = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // chxInstantLookup
            // 
            this.chxInstantLookup.AutoSize = true;
            this.chxInstantLookup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chxInstantLookup.Location = new System.Drawing.Point(0, 390);
            this.chxInstantLookup.Name = "chxInstantLookup";
            this.chxInstantLookup.Size = new System.Drawing.Size(225, 17);
            this.chxInstantLookup.TabIndex = 2;
            this.chxInstantLookup.Text = "Instant lookup";
            this.chxInstantLookup.UseVisualStyleBackColor = true;
            this.chxInstantLookup.Click += new System.EventHandler(this.chxInstantLookup_Click);
            // 
            // btnLookup
            // 
            this.btnLookup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnLookup.Location = new System.Drawing.Point(0, 367);
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.Size = new System.Drawing.Size(225, 23);
            this.btnLookup.TabIndex = 3;
            this.btnLookup.Text = "Look up";
            this.btnLookup.UseVisualStyleBackColor = true;
            this.btnLookup.Click += new System.EventHandler(this.btnLookup_Click);
            // 
            // txtLemma
            // 
            this.txtLemma.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtLemma.Location = new System.Drawing.Point(0, 0);
            this.txtLemma.Name = "txtLemma";
            this.txtLemma.Size = new System.Drawing.Size(225, 20);
            this.txtLemma.TabIndex = 4;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 20);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(225, 347);
            this.listBox1.TabIndex = 5;
            // 
            // LookupPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.txtLemma);
            this.Controls.Add(this.btnLookup);
            this.Controls.Add(this.chxInstantLookup);
            this.Name = "LookupPane";
            this.Size = new System.Drawing.Size(225, 407);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.TextBox txtLemma;
        public System.Windows.Forms.CheckBox chxInstantLookup;
        public System.Windows.Forms.ListBox listBox1;
    }
}
