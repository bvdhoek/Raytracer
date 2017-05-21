﻿
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

        Debugger debugger = new Debugger();

        public Camera camera = new Camera();
        public Scene scene = new Scene();

        // trace a ray for each pixel and draw on the bitmap
        public unsafe Bitmap Render()
        {
            debugger.SetupDebugView(camera, scene);

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
                    Vector3 color = Trace(ray, i == 256 && j % 16 == 0);
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

            Bitmap combined = new Bitmap(1024, 512);
            Graphics combinedGraphics = Graphics.FromImage(combined);
            combinedGraphics.DrawImage(image3D, 0, 0);
            combinedGraphics.DrawImage(debugger.image2D, 512, 0);

            return combined;
        }

        // This is where the magic happens! Trace a ray...
        Vector3 Trace(Ray ray, bool drawDebugLine)
        {
            // ... and see if it hits anything.
            Intersection intersect = scene.Intersect(ray);

            Vector3 color = new Vector3(0, 0, 0);
            if (intersect == null)
            {
                return color;
            }

            // Draw some debug output if it does hit anything.
            if (drawDebugLine)
            {
                debugger.DrawDebugLine(ray.origin.X, ray.origin.Z, intersect.intersectionPoint.X, intersect.intersectionPoint.Z, true);
            }

            if (intersect.primitive.material.isMirror)
            {
                Ray mirrorRay = new Ray() { origin = intersect.intersectionPoint };
                mirrorRay.direction = ray.direction - 2 * intersect.normal * (Vector3.Dot(ray.direction, intersect.normal));
                mirrorRay.origin += 0.01f * mirrorRay.direction;
                color += intersect.primitive.material.reflectiveness * Trace(mirrorRay, true);
            }
            if (intersect.primitive.material.isTransparent)
            {
                // Calculate refraction
                color += intersect.primitive.material.transparency * Trace(ray, false);
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
