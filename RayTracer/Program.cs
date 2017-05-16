using System.Windows.Forms;
using System.Drawing;
using System.Numerics;

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
            this.rayTracer = rayTracer;
            this.Width = 1024;
            this.Height = 512;
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint,
                          true);
            this.UpdateStyles();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Q)
            {
                rayTracer.camera.Zoom(1.1f);
                Invalidate();
            }
            else if (e.KeyCode == Keys.W)
            {
                rayTracer.camera.Zoom(0.9f);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Image image = rayTracer.Render();
            e.Graphics.DrawImage(image, 0, 0);
            base.OnPaint(e);
        }

        private void PlotPixel(Bitmap bitmap, Vector3 color, int i, int j)
        {
            // Plot color to the bitmap using the coördinates
            bitmap.SetPixel(i, j, Color.FromArgb(Clamp((int)(color.X * 255)), Clamp((int)(color.Y * 255)), Clamp((int)(color.Z * 255))));
        }

        int Clamp(int i)
        {
            if (i < 0) i = 0;
            if (i > 255) i = 255;
            return i;
        }
    }
}

