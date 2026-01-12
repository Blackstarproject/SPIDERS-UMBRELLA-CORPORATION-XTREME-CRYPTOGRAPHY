using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SPIDERS_UMBRELLA_CORPORATION.UI.Controls
{
    public class BioProgressBar : Control
    {
        public int Maximum { get; set; } = 100;
        private int _value = 0;
        public int Value { get => _value; set { _value = value; Invalidate(); } }
        public Color LiquidColor { get; set; } = Color.Red;

        public BioProgressBar() { DoubleBuffered = true; BackColor = Color.Black; Height = 30; }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.Black, ClientRectangle);
            g.DrawRectangle(Pens.Gray, 0, 0, Width - 1, Height - 1);
            if (Maximum == 0) return;

            float percent = (float)_value / Maximum;
            int fillWidth = (int)(Width * percent);
            if (fillWidth > 0)
            {
                Rectangle fillRect = new Rectangle(2, 2, fillWidth, Height - 4);
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Point(0, 0), new Point(0, Height), Color.FromArgb(100, LiquidColor), LiquidColor))
                {
                    g.FillRectangle(brush, fillRect);
                }
            }
            using (Pen p = new Pen(Color.FromArgb(50, 255, 255, 255), 2))
            {
                for (int x = 0; x < Width; x += 20)
                {
                    g.DrawLine(p, x, 0, x + 10, Height);
                    g.DrawLine(p, x + 10, 0, x + 20, Height);
                }
            }
        }
    }
}