using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_ComputacionGrafica
{
    public partial class Form1 : Form
    {
        private ReproductorMedia reproductor1;
        private ReproductorEcualizador reproductor2;
        private ReproductorDisco reproductor3;
        private ReproductorOptico reproductor4;

        private int currentIndex = 0;
        private readonly object lockObject = new object();
        private Timer autoTimer;
        private Timer progressTimer;
        private int elapsedMs = 0;
        private const int IntervalMs = 5000;       // Duración de cada animación en ms
        private const int ProgressInterval = 50;    // Actualización de progreso en ms

        public Form1()
        {
            InitializeComponent();

            // Inicializar reproductores
            reproductor1 = new ReproductorMedia(picCanvas);
            reproductor2 = new ReproductorEcualizador(picCanvas);
            reproductor3 = new ReproductorDisco(picCanvas);
            reproductor4 = new ReproductorOptico(picCanvas);

            // Timer para cambio automático de animación
            autoTimer = new Timer { Interval = IntervalMs };
            autoTimer.Tick += AutoTimer_Tick;

            // Timer para barra de progreso
            progressTimer = new Timer { Interval = ProgressInterval };
            progressTimer.Tick += ProgressTimer_Tick;

            // Botones de control
            btnPlay.Click += (s, e) => StartSequence();
            btnPause.Click += (s, e) => PauseCurrent();
            btnResume.Click += (s, e) => ResumeCurrent();
            btnStop.Click += (s, e) => StopSequence();

            // Evento Paint del canvas: primero dibuja animación, luego la barra
            picCanvas.Paint += (s, e) =>
            {
                // Nada aquí: cada reproductor agrega su handler en PlayCurrent
                DrawProgressBar(s, e);
            };
        }

        private void StartSequence()
        {
            lock (lockObject)
            {
                currentIndex = 0;
                elapsedMs = 0;
                PlayCurrent();
                autoTimer.Start();
                progressTimer.Start();
            }
        }

        private void PauseCurrent()
        {
            lock (lockObject)
            {
                GetCurrentPlayer()?.Pause();
                autoTimer.Stop();
                progressTimer.Stop();
                picCanvas.Invalidate();
            }
        }

        private void ResumeCurrent()
        {
            lock (lockObject)
            {
                GetCurrentPlayer()?.Resume();
                autoTimer.Start();
                progressTimer.Start();
                picCanvas.Invalidate();
            }
        }

        private void StopSequence()
        {
            lock (lockObject)
            {
                autoTimer.Stop();
                progressTimer.Stop();
                elapsedMs = 0;
                StopCurrentPlayers();
                DesuscribirPaintHandlers();
                picCanvas.Invalidate();
            }
        }

        private void AutoTimer_Tick(object sender, EventArgs e)
        {
            lock (lockObject)
            {
                currentIndex = (currentIndex + 1) % 4;
                elapsedMs = 0;
                PlayCurrent();
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            elapsedMs = Math.Min(elapsedMs + ProgressInterval, IntervalMs);
            picCanvas.Invalidate(new Rectangle(0, picCanvas.Height - 10, picCanvas.Width, 10));
        }

        private void PlayCurrent()
        {
            StopCurrentPlayers();
            DesuscribirPaintHandlers();

            // Suscribir animación actual
            switch (currentIndex)
            {
                case 0:
                    picCanvas.Paint += reproductor1.DrawAll;
                    reproductor1.Play();
                    break;
                case 1:
                    picCanvas.Paint += reproductor2.OnPaint;
                    reproductor2.Play();
                    break;
                case 2:
                    picCanvas.Paint += reproductor3.DrawAll;
                    reproductor3.Play();
                    break;
                case 3:
                    picCanvas.Paint += reproductor4.OnPaint;
                    reproductor4.Play();
                    break;
            }

            // Luego de la animación, se sobrepone la barra de progreso
            picCanvas.Paint -= DrawProgressBar;
            picCanvas.Paint += DrawProgressBar;

            picCanvas.Invalidate();
        }

        private void DrawProgressBar(object sender, PaintEventArgs e)
        {
            float progress = (float)elapsedMs / IntervalMs;
            int barWidth = (int)(picCanvas.Width * progress);
            var g = e.Graphics;

            // Barra rellena
            using (var brush = new SolidBrush(Color.White))
                g.FillRectangle(brush, 0, picCanvas.Height - 10, barWidth, 10);
            // Contorno
            using (var pen = new Pen(Color.White))
                g.DrawRectangle(pen, 0, picCanvas.Height - 10, picCanvas.Width - 1, 10);
        }

        private dynamic GetCurrentPlayer()
        {
            switch (currentIndex)
            {
                case 0: return reproductor1;
                case 1: return reproductor2;
                case 2: return reproductor3;
                case 3: return reproductor4;
                default: return null;
            }
        }

        private void StopCurrentPlayers()
        {
            reproductor1.Stop();
            reproductor2.Stop();
            reproductor3.Stop();
            reproductor4.Stop();
        }

        private void DesuscribirPaintHandlers()
        {
            picCanvas.Paint -= reproductor1.DrawAll;
            picCanvas.Paint -= reproductor2.OnPaint;
            picCanvas.Paint -= reproductor3.DrawAll;
            picCanvas.Paint -= reproductor4.OnPaint;
        }
    }
}
