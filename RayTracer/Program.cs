﻿using System.Windows.Forms;
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
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Image image = rayTracer.Render();
            e.Graphics.DrawImage(image, 0, 0);
            
            // ♫♫ I see a blank background and i want to paint it black... ♫♫
            e.Graphics.FillRectangle(Brushes.Black, 512, 0, 512, 512);

            // Debug view
            DebugView debugView = new DebugView();
            Image view = (Image) debugView.Render(rayTracer);
            e.Graphics.DrawImage(view, 512, 0, 512, 512);
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
