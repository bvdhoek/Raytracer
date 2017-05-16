using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayTracer
{
    public partial class RaytracerForm : Form
    {
        private RayTracer raytracer;

        public RaytracerForm(RayTracer raytracer)
        {
            this.raytracer = raytracer;
            InitializeComponent();
        }

        private void paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(raytracer.Render(), 10, 10);
        }
    }
}
