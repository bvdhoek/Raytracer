using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
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
            Primitive[] primitives = rayTracer.scene.primitives;

            // Drawsize defines the actual size of the view, so you can add margin!
            int drawsize = 480;
            // Determine the top left corner of the view and flip coordinates so the origin is at the bottom of the screen
            int topleft = 512 - ((bitmap.Height - drawsize) / 2);

            // Camera
            float camX = rayTracer.camera.pos.X + bitmap.Width / 2;
            float camY = topleft + rayTracer.camera.pos.Z;
            graphics.FillRectangle(Brushes.White, camX, camY - 4, 2, 2);

            // Determine furthest object from the camera for scale of the debugview
            float maxX = 0f;
            float maxZ = 0f;
            foreach (Primitive primitive in primitives)
            {
                if (Math.Abs(primitive.origin.X) - Math.Abs(rayTracer.camera.pos.X) + ((Sphere)primitive).r > Math.Abs(maxX))
                {
                    maxX = Math.Abs(primitive.origin.X) - Math.Abs(rayTracer.camera.pos.X) + ((Sphere)primitive).r;
                }

                if (Math.Abs(primitive.origin.Z) - Math.Abs(rayTracer.camera.pos.Z) + ((Sphere)primitive).r > Math.Abs(maxZ))
                {
                    maxZ = Math.Abs(primitive.origin.Z) - Math.Abs(rayTracer.camera.pos.Z) + ((Sphere)primitive).r;
                }
            }

            float max = maxX > maxZ ? maxX : maxZ;
            float scale = drawsize / max;

            // Objects
            foreach (Primitive primitive in primitives)
            {
                if(primitive.GetType() == typeof(Sphere))
                {
                    Sphere sphere = (Sphere)primitive;
                    Color color = Color.FromArgb(
                        (int)primitive.material.color.X * 255, 
                        (int)primitive.material.color.Y * 255, 
                        (int)primitive.material.color.Z * 255
                        );
                    graphics.DrawEllipse(new Pen(color), 
                        (sphere.origin.X - sphere.r / 2) * scale + bitmap.Width / 2, // Half screen offset
                        topleft - (sphere.origin.Z - sphere.r) * scale, 
                        sphere.r * scale, 
                        sphere.r * scale);
                }
            }

            // Screen
            Vector3 p0 = rayTracer.camera.p0;
            Vector3 p1 = rayTracer.camera.p1;
            float offset = bitmap.Width / 2 - p1.X - p0.X;
            float screenX0 = p0.X * scale + offset;
            float screenX1 = p1.X * scale + offset;
            float screenY0 = topleft - p0.Z * scale;
            float screenY1 = topleft - p1.Z * scale;
            graphics.DrawLine(new Pen(Color.White), screenX0, screenY0, screenX1, screenY1);

            return bitmap;
        }
    }
}
