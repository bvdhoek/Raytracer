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
            else if (e.KeyCode == Keys.Left)
            {
                rayTracer.camera.MoveX(-0.1f);
                Invalidate();
            }
            else if (e.KeyCode == Keys.Right)
            {
                rayTracer.camera.MoveX(0.1f);
                Invalidate();
            }
            else if (e.KeyCode == Keys.Up)
            {
                rayTracer.camera.MoveY(0.1f);
                Invalidate();
            }
            else if (e.KeyCode == Keys.Down)
            {
                rayTracer.camera.MoveY(-0.1f);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Image image = rayTracer.Render();
            e.Graphics.DrawImage(image, 0, 0);
        }

        int Clamp(int i)
        {
            if (i < 0) i = 0;
            if (i > 255) i = 255;
            return i;
        }
    }
}

