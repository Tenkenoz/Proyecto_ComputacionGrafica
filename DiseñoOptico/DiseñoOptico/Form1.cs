using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DiseñoOptico
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private int tiempo = 0;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;

            timer = new Timer();
            timer.Interval = 30;
            timer.Tick += (s, e) => { tiempo++; Invalidate(); };
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fondo degradado oscuro
            using (LinearGradientBrush fondo = new LinearGradientBrush(this.ClientRectangle,
                Color.Black, Color.FromArgb(20, 20, 40), LinearGradientMode.Vertical))
            {
                g.FillRectangle(fondo, this.ClientRectangle);
            }

            int ancho = this.ClientSize.Width / 2;
            int alto = this.ClientSize.Height / 2;

            DibujarOndas(g, ancho / 2, alto / 2, "hexagono");         // Superior Izquierdo
            DibujarOndas(g, ancho + ancho / 2, alto / 2, "circulo");  // Superior Derecho
            DibujarOndas(g, ancho / 2, alto + alto / 2, "triangulo"); // Inferior Izquierdo
            DibujarOndas(g, ancho + ancho / 2, alto + alto / 2, "cuadrado"); // Inferior Derecho
        }

        private void DibujarOndas(Graphics g, float cx, float cy, string figura)
        {
            int maxRadio = Math.Max(this.Width, this.Height);
            int spacing = 40;

            for (int i = 0; i < maxRadio / spacing; i++)
            {
                float pulsacion = (float)(10 * Math.Sin((tiempo + i * 10) * 0.05));
                float radio = i * spacing + pulsacion;

                Color color = Color.FromArgb(
                    180,
                    (int)(128 + 127 * Math.Sin((tiempo + i * 5) * 0.02)),
                    (int)(128 + 127 * Math.Sin((tiempo + i * 7) * 0.03)),
                    255
                );

                using (Pen pen = new Pen(color, 2))
                {
                    switch (figura)
                    {
                        case "hexagono":
                            g.DrawPolygon(pen, GenerarPoligono(cx, cy, radio, 6));
                            break;
                        case "circulo":
                            g.DrawEllipse(pen, cx - radio, cy - radio, radio * 2, radio * 2);
                            break;
                        case "triangulo":
                            g.DrawPolygon(pen, GenerarPoligono(cx, cy, radio, 3));
                            break;
                        case "cuadrado":
                            g.DrawPolygon(pen, GenerarPoligono(cx, cy, radio, 4));
                            break;
                    }
                }
            }
        }

        private PointF[] GenerarPoligono(float cx, float cy, float radio, int lados)
        {
            PointF[] puntos = new PointF[lados];
            for (int i = 0; i < lados; i++)
            {
                double angulo = 2 * Math.PI / lados * i - Math.PI / 2;
                float x = cx + (float)(radio * Math.Cos(angulo));
                float y = cy + (float)(radio * Math.Sin(angulo));
                puntos[i] = new PointF(x, y);
            }
            return puntos;
        }
    }
}
