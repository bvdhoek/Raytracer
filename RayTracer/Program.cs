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
            Application.Run(new RaytracerForm(new RayTracer()));
        }
    }
}

