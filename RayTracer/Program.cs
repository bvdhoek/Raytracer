using System.Windows.Forms;
using System.Drawing;

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
            this.Width = 512;
            this.Height = 512;

            this.DrawImage();
        }

        private void DrawImage()
        {
            Graphics g = this.CreateGraphics();
            Image image = (Image)rayTracer.Render();
            g.DrawImage(image, 0, 0, 512, 512);
            this.Invalidate();
        }
    }
}

