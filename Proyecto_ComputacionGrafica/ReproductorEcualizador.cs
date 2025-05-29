using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_ComputacionGrafica
{
    public class ReproductorEcualizador
    {
        private PictureBox canvas;
        private Timer timer;
        private Random rnd = new Random();
        private int barCount = 20;
        private int[] heights;
        private bool isPlaying = false;

        public ReproductorEcualizador(PictureBox pic)
        {
            canvas = pic ?? throw new ArgumentNullException(nameof(pic));
            heights = new int[barCount];
            timer = new Timer { Interval = 100 };
            timer.Tick += (s, e) => UpdateBars();
            // No subscribe Paint here
        }

        public void Play()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                Array.Clear(heights, 0, heights.Length);
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
                Array.Clear(heights, 0, heights.Length);
                canvas.Invalidate();
            }
        }

        private void UpdateBars()
        {
            for (int i = 0; i < barCount; i++)
                heights[i] = rnd.Next(0, canvas.Height);
            canvas.Invalidate();
        }

        /// <summary>
        /// Dibuja el ecualizador en el evento Paint.
        /// </summary>
        public void DrawAll(object sender, PaintEventArgs e)
        {
            if (!isPlaying) return;
            var g = e.Graphics;
            g.Clear(Color.Black);
            int barWidth = canvas.Width / barCount;
            for (int i = 0; i < barCount; i++)
            {
                int x = i * barWidth;
                int h = heights[i];
                var rect = new Rectangle(x, canvas.Height - h, barWidth - 2, h);
                using (var brush = new SolidBrush(Color.Lime))
                    g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Alias para compatibilidad con OnPaint.
        /// </summary>
        public void OnPaint(object sender, PaintEventArgs e) => DrawAll(sender, e);
    }
}



