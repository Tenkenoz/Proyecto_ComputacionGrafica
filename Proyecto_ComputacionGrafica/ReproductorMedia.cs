using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_ComputacionGrafica
{
    internal class ReproductorMedia
    {
        private List<RegularPolygon> polygons;
        private Timer timer;
        private Timer spawnTimer;
        private PictureBox canvas;
        private Random rnd = new Random();

        private const int MAX_POLYGONS = 30;
        private float scaleFactor = 10;
        private float fallSpeed = 4f;
        private float globalRotationSpeed = 2f;
        private bool isPaused = false;
        private bool isPlaying = false;

        public ReproductorMedia(PictureBox picCanvas)
        {
            canvas = picCanvas ?? throw new ArgumentNullException(nameof(picCanvas));
            polygons = new List<RegularPolygon>();

            timer = new Timer { Interval = 30 };
            timer.Tick += Animate;

            spawnTimer = new Timer { Interval = 200 };
            spawnTimer.Tick += SpawnPolygons;

            // No asignar evento Paint aquí
        }

        public void Play()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                isPaused = false;
                polygons.Clear();
                timer.Start();
                spawnTimer.Start();
                canvas.Invalidate();
            }
        }

        public void Pause()
        {
            if (isPlaying && !isPaused)
            {
                isPaused = true;
                timer.Stop();
                spawnTimer.Stop();
            }
        }

        public void Resume()
        {
            if (isPlaying && isPaused)
            {
                isPaused = false;
                timer.Start();
                spawnTimer.Start();
            }
        }

        public void Stop()
        {
            if (isPlaying)
            {
                isPlaying = false;
                isPaused = false;
                timer.Stop();
                spawnTimer.Stop();
                polygons.Clear();
            }
        }

        public void DrawAll(object sender, PaintEventArgs e)
        {
            if (!isPlaying) return;

            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            using (Pen pen = new Pen(Color.Lime, 2))
            {
                foreach (var poly in polygons)
                {
                    PointF[] pts = poly.GetPoints(scaleFactor);
                    if (pts.Length > 1)
                        g.DrawPolygon(pen, pts);
                }
            }
        }

        private void Animate(object sender, EventArgs e)
        {
            foreach (var poly in polygons)
            {
                poly.Y += fallSpeed;
                poly.Angle += globalRotationSpeed;
            }

            polygons.RemoveAll(p => p.Y - p.Size * scaleFactor > canvas.Height);
            canvas.Invalidate();
        }

        private void SpawnPolygons(object sender, EventArgs e)
        {
            if (!isPlaying || isPaused) return;

            int newCount = rnd.Next(1, 4);
            for (int i = 0; i < newCount && polygons.Count < MAX_POLYGONS; i++)
            {
                polygons.Add(CreateRandomPolygon());
            }
        }

        private RegularPolygon CreateRandomPolygon()
        {
            int sides = rnd.Next(3, 8);
            float size = rnd.Next(2, 6);
            float margin = size * scaleFactor;
            float x = rnd.Next((int)margin, canvas.Width - (int)margin);
            float y = -margin;

            return new RegularPolygon(x, y, size, sides);
        }

        private class RegularPolygon
        {
            public float X, Y, Size, Angle;
            public int Sides;

            public RegularPolygon(float x, float y, float size, int sides)
            {
                if (sides < 3)
                    throw new ArgumentException("El número de lados debe ser al menos 3.", nameof(sides));

                X = x;
                Y = y;
                Size = size;
                Sides = sides;
                Angle = 0f;
            }

            public PointF[] GetPoints(float scale)
            {
                PointF[] points = new PointF[Sides];
                double initialAngle = Angle * Math.PI / 180.0;

                for (int i = 0; i < Sides; i++)
                {
                    double theta = initialAngle + i * 2 * Math.PI / Sides;
                    float px = X + Size * scale * (float)Math.Cos(theta);
                    float py = Y + Size * scale * (float)Math.Sin(theta);
                    points[i] = new PointF(px, py);
                }

                return points;
            }
        }
    }
}