using System.Windows.Forms;
using System.Drawing;
using System.Numerics;
using System;

namespace RayTracer
// dist = distance
// d = direction
// r = radius
// o = origin
// pos = position
// l = length
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Run(new RayTracerForm(new RayTracer()));
        }
    }

    public class RayTracerForm : Form
    {
        private RayTracer rayTracer;

        public RayTracerForm(RayTracer rayTracer)
        {
            Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
            this.rayTracer = rayTracer;
            Width = 1024;
            Height = 512 + screenRectangle.Top - this.Top;
            SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint,
                          true);
            UpdateStyles();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Z:
                    rayTracer.camera.Zoom(1.1f);
                    break;
                case Keys.X:
                    rayTracer.camera.Zoom(0.9f);
                    break;
                case Keys.W:
                    rayTracer.camera.MoveForward();
                    break;
                case Keys.A:
                    rayTracer.camera.MoveLeft();
                    break;
                case Keys.S:
                    rayTracer.camera.MoveBack();
                    break;
                case Keys.D:
                    rayTracer.camera.MoveRight();
                    break;
                case Keys.Q:
                    rayTracer.camera.MoveDown();
                    break;
                case Keys.E:
                    rayTracer.camera.MoveUp();
                    Debugger.Reset();
                    break;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // ♫♫ I see a blank background and i want to paint it black... ♫♫
            e.Graphics.FillRectangle(Brushes.Black, 512, 0, 512, 512);

            Image image = rayTracer.Render();
            e.Graphics.DrawImage(image, 0, 0);
            base.OnPaint(e);
        }

        int Clamp(int i)
        {
            if (i < 0) i = 0;
            if (i > 255) i = 255;
            return i;
        }
    }
}
