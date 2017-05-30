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
                    rayTracer.camera.Move(0.1f, rayTracer.camera.d);
                    break;
                case Keys.A:
                    rayTracer.camera.Move(-0.1f, rayTracer.camera.right);
                    break;
                case Keys.S:
                    rayTracer.camera.Move(-0.1f, rayTracer.camera.d);
                    break;
                case Keys.D:
                    rayTracer.camera.Move(0.1f, rayTracer.camera.right);
                    break;
                case Keys.Q:
                    rayTracer.camera.Move(-0.1f, rayTracer.camera.up);
                    break;
                case Keys.E:
                    rayTracer.camera.Move(0.1f, rayTracer.camera.up);
                    break;
                case Keys.Right:
                    rayTracer.camera.RotateRight(2);
                    break;
                case Keys.Left:
                    rayTracer.camera.RotateLeft(2);
                    break;
                case Keys.Up:
                    rayTracer.camera.RotateUp(2);
                    break;
                case Keys.Down:
                    rayTracer.camera.RotateDown(2);
                    break;
                case Keys.M:
                    rayTracer.camera.TurnRight(2);
                    break;
                case Keys.N:
                    rayTracer.camera.TurnLeft(2);
                    break;
            }
            Console.WriteLine("Camera position: ({0}, {1}, {2})", rayTracer.camera.pos.X, rayTracer.camera.pos.Y, rayTracer.camera.pos.Z);
            Console.WriteLine("Camera direction: ({0}, {1}, {2})", rayTracer.camera.d.X, rayTracer.camera.d.Y, rayTracer.camera.d.Z);
            Console.WriteLine("direction length: {0}", rayTracer.camera.d.Length());
            Console.WriteLine("Camera up: ({0}, {1}, {2})", rayTracer.camera.up.X, rayTracer.camera.up.Y, rayTracer.camera.up.Z);
            Console.WriteLine("up length: {0}", rayTracer.camera.up.Length());
            Console.WriteLine("Camera right: ({0}, {1}, {2})", rayTracer.camera.right.X, rayTracer.camera.right.Y, rayTracer.camera.right.Z);
            Console.WriteLine("right length: {0}", rayTracer.camera.right.Length());
            Console.WriteLine("");
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
