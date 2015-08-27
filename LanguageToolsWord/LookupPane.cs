using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LanguageTools.Backend;

namespace LanguageTools.Word
{
    public partial class LookupPane : UserControl
    {
        private Lemma item = new Lemma("", Lemma.WordGender.Neutral);
        public Lemma Item
        {
            get { return item; }
            set
            {
                item = value;
                UpdateItemValues();
            }
        }

        public LookupPane()
        {
            InitializeComponent();
            UpdateItemValues();
        }

        private void UpdateItemValues()
        {
            txtLemma.Text = Item.Text + "{" + Item.Gender.ToString() + "}";
        }

        private void btnLookup_Click(object sender, EventArgs e)
        {
            Globals.ThisAddIn.LookupValue(txtLemma.Text);
        }

        private void chxInstantLookup_Click(object sender, EventArgs e)
        {
            Globals.ThisAddIn.ToggleInstantLookup(((CheckBox)sender).Checked);
        }
    }
}
