namespace LanguageTools.Common
{
    partial class LanguageToolsRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private LanguageToolsRibbonListener listener;

        public LanguageToolsRibbon(Microsoft.Office.Tools.Ribbon.RibbonFactory factory, LanguageToolsRibbonListener listener)
            : base(factory)
        {
            InitializeComponent();
            this.listener = listener;
        }

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
            this.tabLanguageTools = this.Factory.CreateRibbonTab();
            this.grpGermanGrammar = this.Factory.CreateRibbonGroup();
            this.btnLookup = this.Factory.CreateRibbonButton();
            this.btnToggleLookupPane = this.Factory.CreateRibbonToggleButton();
            this.btnToggleInstantLookup = this.Factory.CreateRibbonToggleButton();
            this.tabLanguageTools.SuspendLayout();
            this.grpGermanGrammar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabLanguageTools
            // 
            this.tabLanguageTools.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabLanguageTools.Groups.Add(this.grpGermanGrammar);
            this.tabLanguageTools.Label = "Language Tools";
            this.tabLanguageTools.Name = "tabLanguageTools";
            // 
            // grpGermanGrammar
            // 
            this.grpGermanGrammar.Items.Add(this.btnLookup);
            this.grpGermanGrammar.Items.Add(this.btnToggleLookupPane);
            this.grpGermanGrammar.Items.Add(this.btnToggleInstantLookup);
            this.grpGermanGrammar.Label = "German Grammar";
            this.grpGermanGrammar.Name = "grpGermanGrammar";
            // 
            // btnLookup
            // 
            this.btnLookup.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLookup.Image = Properties.Resources.Search_icon;
            this.btnLookup.Label = "Look up";
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.ScreenTip = "Search selection in dictionary";
            this.btnLookup.ShowImage = true;
            this.btnLookup.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLookup_Click);
            // 
            // btnToggleLookupPane
            // 
            this.btnToggleLookupPane.Image = Properties.Resources.panel_icon;
            this.btnToggleLookupPane.Label = "Lookup pane";
            this.btnToggleLookupPane.Name = "btnToggleLookupPane";
            this.btnToggleLookupPane.ShowImage = true;
            this.btnToggleLookupPane.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnToggleLookupPane_Click);
            // 
            // btnToggleInstantLookup
            // 
            this.btnToggleInstantLookup.Image = Properties.Resources.Search_icon2;
            this.btnToggleInstantLookup.Label = "Instant lookup";
            this.btnToggleInstantLookup.Name = "btnToggleInstantLookup";
            this.btnToggleInstantLookup.ShowImage = true;
            this.btnToggleInstantLookup.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnToggleInstantLookup_Click);
            // 
            // LanguageToolsRibbon
            // 
            this.Name = "LanguageToolsRibbon";
            this.RibbonType = "Microsoft.Outlook.Mail.Compose, Microsoft.Word.Document";
            this.Tabs.Add(this.tabLanguageTools);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.LanguageToolsRibbon_Load);
            this.tabLanguageTools.ResumeLayout(false);
            this.tabLanguageTools.PerformLayout();
            this.grpGermanGrammar.ResumeLayout(false);
            this.grpGermanGrammar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public Microsoft.Office.Tools.Ribbon.RibbonTab tabLanguageTools;
        public Microsoft.Office.Tools.Ribbon.RibbonGroup grpGermanGrammar;
        public Microsoft.Office.Tools.Ribbon.RibbonToggleButton btnToggleLookupPane;
        public Microsoft.Office.Tools.Ribbon.RibbonButton btnLookup;
        public Microsoft.Office.Tools.Ribbon.RibbonToggleButton btnToggleInstantLookup;
    }

}
