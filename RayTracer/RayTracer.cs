
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

        public Camera camera = new Camera();
        Scene scene = new Scene();

        // trace a ray for each pixel and draw on the bitmap
        public unsafe Bitmap Render()
        {
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
                    float y = ((float)j) / 512;
                    Ray ray = camera.MakeRay(x, y);
                    Vector3 color = Trace(ray);
                    *line++ = (uint) Color.FromArgb(
                                        Clamp((int)(color.X * 255)),
                                        Clamp((int)(color.Y * 255)),
                                        Clamp((int)(color.Z * 255))
                                     ).ToArgb();
                }
                // Set pointer to next line
                ptr += bitmapData.Stride / 4;
            }

            // Unlock the bits.
            image3D.UnlockBits(bitmapData);

            return image3D;
        }

        Vector3 Trace(Ray ray)
        {
            Intersection intersect = scene.Intersect(ray);
            if (intersect == null)
            {
                return new Vector3(0, 0, 0);
            }
            if (intersect.primitive.material.isMirror)
            { } // Not implemented yet; cast a mirror ray:
            //   return intersect.primitive.material.color * Trace( );
            return scene.DirectIllumination(intersect) * intersect.primitive.material.color;
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
