
//Raytracer, which owns the scene, camera and the display surface.The Raytracer implements a
//method Render, which uses the camera to loop over the pixels of the screen plane and to
//generate a ray for each pixel, which is then used to find the nearest intersection.The result is
//then visualized by plotting a pixel. For one line of pixels (typically line 256 for a 512x512
//window), it generates debug output by visualizing every Nth ray(where N is e.g. 10).

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace RayTracer
{
    public class RayTracer
    {
        Bitmap image3D = new Bitmap(512, 512);
        Bitmap image2D = new Bitmap(512, 512);

        public Camera camera = new Camera();
        public Scene scene = new Scene();

        float screenX0, screenX1, screenZ0, screenZ1, camX, camZ;
        Graphics graphics2D;

        // trace a ray for each pixel and draw on the bitmap
        public unsafe Bitmap Render()
        {
            graphics2D = Graphics.FromImage(image2D);

            // Debugview
            Primitive[] primitives = scene.primitives;

            // Drawsize defines the actual size of the view, so you can add margin!
            int drawsize = 450;
            // Determine the top left corner of the view and flip coordinates so the origin is at the bottom of the screen
            int topZ = 512 - ((image2D.Height - drawsize) / 2);

            // Camera
            camX = camera.pos.X + image2D.Width / 2;
            camZ = topZ + camera.pos.Z;
            graphics2D.FillRectangle(Brushes.White, camX, camZ - 4, 2, 2);

            // Determine furthest object from the camera for scale of the debugview
            float maxX = 0f;
            float maxZ = 0f;
            foreach (Primitive primitive in primitives)
            {
                if (Math.Abs(primitive.origin.X) - Math.Abs(camera.pos.X) + ((Sphere)primitive).r > Math.Abs(maxX))
                {
                    maxX = Math.Abs(primitive.origin.X) - Math.Abs(camera.pos.X) + ((Sphere)primitive).r;
                }

                if (Math.Abs(primitive.origin.Z) - Math.Abs(camera.pos.Z) + ((Sphere)primitive).r > Math.Abs(maxZ))
                {
                    maxZ = Math.Abs(primitive.origin.Z) - Math.Abs(camera.pos.Z) + ((Sphere)primitive).r;
                }
            }

            float max = maxX > maxZ ? maxX : maxZ;
            float scale = drawsize / max;

            // Objects
            foreach (Primitive primitive in primitives)
            {
                if (primitive.GetType() == typeof(Sphere))
                {
                    Sphere sphere = (Sphere)primitive;
                    Color color = Color.FromArgb(
                        (int)primitive.material.color.X * 255,
                        (int)primitive.material.color.Y * 255,
                        (int)primitive.material.color.Z * 255
                        );
                    graphics2D.DrawEllipse(new Pen(color),
                        (sphere.origin.X - sphere.r / 2) * scale + image2D.Width / 2, // center drawing
                        topZ - (sphere.origin.Z - sphere.r) * scale,
                        sphere.r * scale,
                        sphere.r * scale);
                }
            }

            // Screen
            Vector3 p0 = camera.p0;
            Vector3 p1 = camera.p1;
            float offset = image2D.Width / 2 - p1.X - p0.X;
            screenX0 = p0.X * scale + offset;
            screenX1 = p1.X * scale + offset;
            screenZ0 = topZ - p0.Z * scale;
            screenZ1 = topZ - p1.Z * scale;
            graphics2D.DrawLine(new Pen(Color.White), screenX0, screenZ0, screenX1, screenZ1);

            // 3D image
            Rectangle rect = new Rectangle(0, 0, 512, 512);
            // lock the bits
            BitmapData bitmapData =
                image3D.LockBits(rect, ImageLockMode.WriteOnly,
                PixelFormat.Format32bppPArgb);

            // Get the address of the first line.
            uint* ptr = (uint*)bitmapData.Scan0;

            for (int i = 0; i < 512; i++)
            {
                uint* line = ptr;

                for (int j = 0; j < 512; j++)
                {
                    float x = ((float)j) / 512;
                    float y = ((float)i) / 512;
                    Ray ray = camera.MakeRay(x, y);
                    Vector3 color = Trace(ray);
                    *line++ = (uint) Color.FromArgb(
                                        Clamp((int)(color.X * 255)),
                                        Clamp((int)(color.Y * 255)),
                                        Clamp((int)(color.Z * 255))
                                     ).ToArgb();
                    if(j % 32 == 0)
                    {
                        DebugLine(j);
                    }
                }
                // Set pointer to next line
                ptr += bitmapData.Stride / 4;
            }

            // Unlock the bits.
            image3D.UnlockBits(bitmapData);

            Bitmap combined = new Bitmap(1024, 512);
            Graphics combinedGraphics = Graphics.FromImage(combined);
            combinedGraphics.DrawImage(image3D, 0, 0);
            combinedGraphics.DrawImage(image2D, 512, 0);

            return combined;
        }

        private void DebugLine(float count)
        {
            graphics2D.DrawLine(new Pen(Color.Yellow), camX, camZ, 0, screenZ0);
        }

        Vector3 Trace(Ray ray)
        {
            Intersection intersect = scene.Intersect(ray);
            Vector3 color = new Vector3(0, 0, 0);

            if (intersect == null)
            {
                return color;
            }

            if (intersect.primitive.material.isMirror)
            {
                Ray mirrorRay = new Ray() { origin = intersect.intersectionPoint };
                mirrorRay.direction = ray.direction - 2 * intersect.normal * (Vector3.Dot(ray.direction, intersect.normal));
                mirrorRay.origin += 0.01f * mirrorRay.direction;
                color += intersect.primitive.material.reflectiveness * Trace(mirrorRay);
            }
            if (intersect.primitive.material.isTransparent)
            {
                // Calculate refraction
                color += intersect.primitive.material.transparency * Trace(ray);
            }
            if(intersect.primitive.material.isShiny)
            {
                // Calculate reflection of shiny object
            }
            color += intersect.primitive.material.diffuseness * scene.DirectIllumination(intersect) * intersect.primitive.material.color;
            return color;
        }

        // Clamp integer to minimum 0 and max 255
        int Clamp(int i)
        {
            if (i < 0) i = 0;
            if (i > 255) i = 255;
            return i;
        }
    }
}
