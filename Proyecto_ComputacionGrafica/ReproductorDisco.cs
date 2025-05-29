using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_ComputacionGrafica
{
    public class ReproductorDisco
    {
        private PictureBox canvas;
        private Timer timer;
        private float angle = 0f;
        private const int BAR_COUNT = 30;
        private const float ROTATION_SPEED = 3f;
        private const int MAX_BAR_LENGTH = 100;
        private bool isPlaying = false;
        private Random rnd = new Random();
        private float[] barLengths;

        public ReproductorDisco(PictureBox picCanvas)
        {
            canvas = picCanvas ?? throw new ArgumentNullException(nameof(picCanvas));
            timer = new Timer { Interval = 50 }; // ~20fps
            timer.Tick += Rotate;
            barLengths = new float[BAR_COUNT];
        }

        public void Play()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                angle = 0f;
                for (int i = 0; i < BAR_COUNT; i++)
                    barLengths[i] = rnd.Next(MAX_BAR_LENGTH / 2, MAX_BAR_LENGTH);
                timer.Start();
                canvas.Invalidate();
            }
        }

        public void Pause()
        {
            if (isPlaying) timer.Stop();
        }

        public void Resume()
        {
            if (isPlaying) timer.Start();
        }

        public void Stop()
        {
            if (isPlaying)
            {
                isPlaying = false;
                timer.Stop();
                angle = 0f;
                canvas.Invalidate();
            }
        }

        private void Rotate(object sender, EventArgs e)
        {
            angle += ROTATION_SPEED;
            if (angle >= 360f) angle -= 360f;
            // aleatoriza longitudes ligeramente
            for (int i = 0; i < BAR_COUNT; i++)
                barLengths[i] = MAX_BAR_LENGTH / 2 + (float)(Math.Sin((angle + i * 12) * Math.PI / 180) * (MAX_BAR_LENGTH / 2));
            canvas.Invalidate();
        }

        public void DrawAll(object sender, PaintEventArgs e)
        {
            if (!isPlaying) return;
            var g = e.Graphics;
            g.Clear(Color.Black);
            var center = new PointF(canvas.Width / 2f, canvas.Height / 2f);
            float deltaAngle = 360f / BAR_COUNT;
            using (var pen = new Pen(Color.Orange, 4))
            {
                for (int i = 0; i < BAR_COUNT; i++)
                {
                    float a = angle + i * deltaAngle;
                    double rad = a * Math.PI / 180;
                    float len = barLengths[i];
                    var end = new PointF(
                        center.X + len * (float)Math.Cos(rad),
                        center.Y + len * (float)Math.Sin(rad));
                    g.DrawLine(pen, center, end);
                }
            }
        }

        public void OnPaint(object sender, PaintEventArgs e) => DrawAll(sender, e);
    }
}
