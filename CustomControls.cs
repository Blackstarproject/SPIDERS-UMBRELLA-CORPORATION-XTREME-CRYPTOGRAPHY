using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace SPIDERS_UMBRELLA_CORPORATION.UI.Controls
{
    // 1. Breathing Button 
    public class UmbrellaButton : Button
    {
        private readonly Timer _pulseTimer;
        private int _alpha = 100;
        private bool _increasing = true;
        private bool _isHovered = false;

        public UmbrellaButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = Color.White;
            Font = new Font("Arial", 12, FontStyle.Bold);
            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            _pulseTimer = new Timer { Interval = 30 };
            _pulseTimer.Tick += (s, e) => {
                if (_increasing) _alpha += 5; else _alpha -= 5;
                if (_alpha >= 255) _increasing = false;
                if (_alpha <= 100) _increasing = true;
                Invalidate();
            };
            _pulseTimer.Start();
        }
        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _isHovered = true; }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _isHovered = false; }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.Clear(Color.Black);
            Color baseColor = BackColor;
            int alpha = _isHovered ? 255 : _alpha;
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, baseColor))) g.FillRectangle(brush, ClientRectangle);
            using (Pen p = new Pen(baseColor, 2)) g.DrawRectangle(p, 1, 1, Width - 2, Height - 2);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    // 2. 3D DNA Helix Spinner 
    public class DnaHelixSpinner : Control
    {
        private readonly Timer _animTimer;
        private float _phase = 0;
        public Color HelixColor { get; set; } = Color.Red;

        public DnaHelixSpinner()
        {
            DoubleBuffered = true;
            BackColor = Color.Black;
            _animTimer = new Timer { Interval = 20 };
            _animTimer.Tick += (s, e) => { _phase += 0.15f; Invalidate(); };
            _animTimer.Start();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            float amp = Width / 3f;
            float centerX = Width / 2f;
            float freq = 0.08f;

            // Draw back strands first, then front
            for (int layer = 0; layer < 2; layer++) // 0 = back, 1 = front
            {
                for (int y = 0; y < Height; y += 8)
                {
                    float sine1 = (float)Math.Sin(y * freq + _phase);
                    float sine2 = (float)Math.Sin(y * freq + _phase + Math.PI);

                    DrawNode(g, layer, centerX, y, amp, sine1);
                    DrawNode(g, layer, centerX, y, amp, sine2);

                    // Draw connecting lines only on back layer render pass for cleanliness
                    if (layer == 0)
                    {
                        float x1 = centerX + sine1 * amp;
                        float x2 = centerX + sine2 * amp;
                        using (Pen p = new Pen(Color.FromArgb(50, Color.Gray))) g.DrawLine(p, x1, y, x2, y);
                    }
                }
            }
        }
        private void DrawNode(Graphics g, int layer, float cx, float y, float amp, float sineVal)
        {
            bool isFront = sineVal > 0;
            if ((layer == 0 && isFront) || (layer == 1 && !isFront)) return;

            float x = cx + sineVal * amp;
            int alpha = (int)(100 + (sineVal + 1) * 77); // 100-255
            float size = 4 + (sineVal + 1) * 3; // Scale 4-10

            using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, HelixColor)))
                g.FillEllipse(b, x - size / 2, y - size / 2, size, size);
        }
    }

    // 3. Sonar Radar
    public class SonarRadar : Control
    {
        private readonly Timer _sweepTimer;
        private float _angle = 0;
        private readonly List<PointF> _blips = new List<PointF>();
        private readonly Random _rnd = new Random();

        public SonarRadar()
        {
            DoubleBuffered = true;
            BackColor = Color.Black;
            _sweepTimer = new Timer { Interval = 20 };
            _sweepTimer.Tick += (s, e) => { _angle = (_angle + 3) % 360; Invalidate(); };
            _sweepTimer.Start();
        }

        public void AddBlip()
        {
            // Add a blip at random radius/angle
            float r = _rnd.Next(10, Width / 2 - 5);
            float a = _rnd.Next(0, 360);
            float rad = (float)(Math.PI / 180 * a);
            float cx = Width / 2;
            float cy = Height / 2;
            lock (_blips) _blips.Add(new PointF(cx + (float)Math.Cos(rad) * r, cy + (float)Math.Sin(rad) * r));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            float cx = Width / 2f; float cy = Height / 2f; float r = Math.Min(Width, Height) / 2f - 2;

            // Draw Rings
            using (Pen p = new Pen(Color.DarkGreen))
            {
                g.DrawEllipse(p, cx - r, cy - r, r * 2, r * 2);
                g.DrawEllipse(p, cx - r / 2, cy - r / 2, r, r);
                g.DrawLine(p, cx, cy - r, cx, cy + r);
                g.DrawLine(p, cx - r, cy, cx + r, cy);
            }

            // Draw Blips
            lock (_blips)
            {
                for (int i = _blips.Count - 1; i >= 0; i--)
                {
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(200, 255, 0, 0)))
                        g.FillEllipse(b, _blips[i].X - 2, _blips[i].Y - 2, 5, 5);
                    // Decay logic would go here in a real update loop, simpler just to clear periodically or keep minimal
                    if (_rnd.Next(0, 20) == 0) _blips.RemoveAt(i);
                }
            }

            // Draw Sweep
            using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, Color.Empty, Color.Empty, 0, false))
            {
                ColorBlend cb = new ColorBlend
                {
                    Positions = new float[] { 0, 1 },
                    Colors = new Color[] { Color.Transparent, Color.FromArgb(100, 0, 255, 0) }
                };
                brush.InterpolationColors = cb;
                brush.RotateTransform(_angle);
                brush.TranslateTransform(cx, cy, MatrixOrder.Append);

                GraphicsPath path = new GraphicsPath();
                path.AddPie(cx - r, cy - r, r * 2, r * 2, _angle - 45, 45);
                g.FillPath(brush, path);
            }
        }
    }

    // 4. EKG Monitor 
    public class EkgMonitor : Control
    {
        private readonly List<float> _history = new List<float>();
        private readonly Timer _timer;
        private readonly Random _rnd = new Random();
        private bool _spiking = false;

        public EkgMonitor()
        {
            DoubleBuffered = true;
            BackColor = Color.Black;
            for (int i = 0; i < 100; i++) _history.Add(0.5f);
            _timer = new Timer { Interval = 30 };
            _timer.Tick += (s, e) => UpdateGraph();
            _timer.Start();
        }

        public void TriggerSpike() => _spiking = true;

        private void UpdateGraph()
        {
            float val = 0.5f; // Baseline
            if (_spiking)
            {
                val = (float)_rnd.NextDouble();
                _spiking = false;
            }
            _history.Add(val);
            if (_history.Count > Width / 5) _history.RemoveAt(0);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Grid
            using (Pen p = new Pen(Color.FromArgb(30, 0, 255, 0)))
            {
                for (int x = 0; x < Width; x += 20) g.DrawLine(p, x, 0, x, Height);
                for (int y = 0; y < Height; y += 20) g.DrawLine(p, 0, y, Width, y);
            }

            // Line
            if (_history.Count < 2) return;
            PointF[] points = new PointF[_history.Count];
            for (int i = 0; i < _history.Count; i++)
            {
                points[i] = new PointF(i * 5, Height - (_history[i] * Height));
            }
            using (Pen p = new Pen(Color.Lime, 2)) g.DrawLines(p, points);
        }
    }
}