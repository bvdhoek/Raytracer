using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
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
            this.rayTracer = rayTracer;
            this.Width = 1024;
            this.Height = 512;           
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Image image = (Image)rayTracer.Render();
            e.Graphics.DrawImage(image, 0, 0, 512, 512);
            
            // ♫♫ I see a blank background and i want to paint it black... ♫♫
            e.Graphics.FillRectangle(Brushes.Black, 512, 0, 512, 512);

            // Debug view
            DebugView debugView = new DebugView();
            Image view = (Image) debugView.Render();
            e.Graphics.DrawImage(view, 512, 0, 512, 512);

            base.OnPaint(e);
        }
    }
}

