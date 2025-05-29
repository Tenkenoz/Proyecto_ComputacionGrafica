using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Proyecto_ComputacionGrafica
{
    public class ReproductorOptico
    {
        private Timer timer;
        private int tiempo = 0;
        private Control contenedor;
        private bool isPaused = false;
        private bool isPlaying = false;

        public ReproductorOptico(Control contenedor)
        {
            this.contenedor = contenedor ?? throw new ArgumentNullException(nameof(contenedor));
            timer = new Timer { Interval = 30 };
            timer.Tick += (s, e) => {
                if (isPlaying && !isPaused)
                {
                    tiempo++;
                    contenedor.Invalidate();
                }
            };
        }

        public void Play()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                isPaused = false;
                tiempo = 0;
                timer.Start();
                contenedor.Invalidate();
            }
        }

        public void Pause()
        {
            if (isPlaying && !isPaused)
            {
                isPaused = true;
            }
        }

        public void Resume()
        {
            if (isPlaying && isPaused)
            {
                isPaused = false;
            }
        }

        public void Stop()
        {
            if (isPlaying)
            {
                isPlaying = false;
                isPaused = false;
                timer.Stop();
                tiempo = 0;
            }
        }

        public void OnPaint(object sender, PaintEventArgs e)
        {
            if (!isPlaying) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (LinearGradientBrush fondo = new LinearGradientBrush(contenedor.ClientRectangle,
                Color.Black, Color.FromArgb(20, 20, 40), LinearGradientMode.Vertical))
            {
                g.FillRectangle(fondo, contenedor.ClientRectangle);
            }

            int ancho = contenedor.ClientSize.Width / 2;
            int alto = contenedor.ClientSize.Height / 2;

            DibujarOndas(g, ancho / 2, alto / 2, "hexagono");
            DibujarOndas(g, ancho + ancho / 2, alto / 2, "circulo");
            DibujarOndas(g, ancho / 2, alto + alto / 2, "triangulo");
            DibujarOndas(g, ancho + ancho / 2, alto + alto / 2, "cuadrado");
        }

        private void DibujarOndas(Graphics g, float cx, float cy, string figura)
        {
            int maxRadio = Math.Max(contenedor.Width, contenedor.Height);
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