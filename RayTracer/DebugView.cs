using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace RayTracer
{
    class DebugView
    {
        Bitmap bitmap = new Bitmap(512, 512);

        public Bitmap Render(RayTracer rayTracer)
        {
            // Time to draw some items!
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawRectangle(new Pen(Color.White), 255, 448, 2, 2);
            Primitive[] primitives = rayTracer.scene.primitives;
            foreach(Primitive primitive in primitives)
            {
                if(primitive.GetType() == typeof(Sphere))
                {
                    Color color = Color.FromArgb(
                        (int)primitive.material.color.X * 255, 
                        (int)primitive.material.color.Y * 255, 
                        (int)primitive.material.color.Z * 255
                        );
                    graphics.DrawEllipse(new Pen(color), 224, 64, 64, 64);
                }
            }

            return bitmap;
        }
    }
}
