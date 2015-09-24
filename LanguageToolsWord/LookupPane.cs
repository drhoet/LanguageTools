using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LanguageTools.Word {
    public partial class LookupPane : UserControl {
        private Lemma item;
        public Lemma Item {
            get { return item; }
            set {
                item = value;
                if(item != null) {
                    txtLemma.Text = Item.Word;
                }
            }
        }

        public SynchronizationContext SyncCtx { get; private set; }

        private Font defaultHeaderFont, defaultTextFont;
        private FontFamily headerFontFamily = new FontFamily("times new roman");

        public LookupPane() {
            InitializeComponent();
            defaultHeaderFont = new Font(headerFontFamily, 25, FontStyle.Bold, GraphicsUnit.Pixel);
            defaultTextFont = new Font(headerFontFamily, 10, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private void btnLookup_Click(object sender, EventArgs e) {
            Globals.ThisAddIn.LookupValue(txtLemma.Text);
        }

        private void chxInstantLookup_Click(object sender, EventArgs e) {
            Globals.ThisAddIn.ToggleInstantLookup(((CheckBox)sender).Checked);
        }

        private void lbxResults_DrawItem(object sender, DrawItemEventArgs e) {
            Lemma lemma = (Lemma)lbxResults.Items[e.Index];
            e.DrawBackground();
            Graphics g = e.Graphics;
            Color bgColor;
            switch(lemma.Gender) {
                case Lemma.WordGender.Mannlich: bgColor = Color.SkyBlue; break;
                case Lemma.WordGender.Weiblich: bgColor = Color.Pink; break;
                case Lemma.WordGender.Neutrum: bgColor = Color.Gold; break;
                default: bgColor = Color.LightGoldenrodYellow; break;
            }
            g.FillRectangle(new SolidBrush(bgColor), e.Bounds);

            float scaleFactor = FindFontScaleHor(g, string.Format("{0} ({1})", lemma.Word, WordGenderConvert.ToString(lemma.Gender)), defaultHeaderFont, e.Bounds.Width);
            Font lemmaHeaderFont = new Font(headerFontFamily, 25 * scaleFactor, FontStyle.Bold, GraphicsUnit.Pixel);
            Font lemmaGenderFont = new Font(headerFontFamily, 15 * scaleFactor, FontStyle.Italic, GraphicsUnit.Pixel);

            int headerHeight = GetFontLineHeight(lemmaHeaderFont);
            Rectangle lemmaRect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, headerHeight);
            lemmaRect.Width = Convert.ToInt32(Math.Ceiling(g.MeasureString(lemma.Word, lemmaHeaderFont).Width));
            g.DrawString(lemma.Word, lemmaHeaderFont, Brushes.LightSlateGray, lemmaRect);

            int genderOffset = GetFontAscent(lemmaHeaderFont) - GetFontAscent(lemmaGenderFont);
            int genderHeight = GetFontLineHeight(lemmaGenderFont);
            Rectangle genderRect = new Rectangle(lemmaRect.Width, e.Bounds.Y + genderOffset, e.Bounds.Width - lemmaRect.Width, genderHeight);
            g.DrawString(string.Format("({0})", WordGenderConvert.ToString(lemma.Gender)), lemmaGenderFont, Brushes.LightSlateGray, genderRect);

            e.DrawFocusRectangle();
        }

        private float FindFontScaleHor(Graphics g, string text, Font preferredFont, int maxWith) {
            SizeF realSize = g.MeasureString(text, preferredFont);
            if(realSize.Width > maxWith) {
                return maxWith / realSize.Width;
            } else {
                return 1;
            }
        }

        private int GetFontAscent(Font font) {
            return Convert.ToInt32(GetDesignToPixelFactor(font) * headerFontFamily.GetCellAscent(font.Style));
        }

        private int GetFontLineHeight(Font font) {
            return Convert.ToInt32(GetDesignToPixelFactor(font) * headerFontFamily.GetLineSpacing(font.Style));
        }

        private float GetDesignToPixelFactor(Font font) {
            return font.Size / font.FontFamily.GetEmHeight(font.Style);
        }

        private void LookupPane_Load(object sender, EventArgs e) {
            SyncCtx = SynchronizationContext.Current;
        }

        private void lbxResults_MeasureItem(object sender, MeasureItemEventArgs e) {
            Lemma lemma = (Lemma)lbxResults.Items[e.Index];

            float scaleFactor = FindFontScaleHor(e.Graphics, string.Format("{0} ({1})", lemma.Word, WordGenderConvert.ToString(lemma.Gender)), defaultHeaderFont, lbxResults.Width);
            Font lemmaHeaderFont = new Font(headerFontFamily, 25 * scaleFactor, FontStyle.Bold, GraphicsUnit.Pixel);
            Font lemmaGenderFont = new Font(headerFontFamily, 15 * scaleFactor, FontStyle.Italic, GraphicsUnit.Pixel);

            e.ItemHeight = Convert.ToInt32(GetFontLineHeight(lemmaHeaderFont) * 1.15);
        }
    }
}