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
        static int screenSize = 512;
        Bitmap image3D = new Bitmap(512, 512);

        Debugger debugger = new Debugger();

        public Camera camera = new Camera();
        public Scene scene = new Scene();

        // How many rays are cast per pixel; implementation of anti-aliasing.
        private int raysPerPixel = 10;

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

            Random r = new Random();
            // Get the address of the first line.
            uint* ptr = (uint*)bitmapData.Scan0;

            // Divide by 265 in stead of 512 to make it a little bit noisy; it looks better.
            float pixelFraction = ((float)1/265) / raysPerPixel;

            for (int i = 0; i < 512; i++)
            {
                uint* line = ptr;

                for (int j = 0; j < 512; j++)
                {
                    Vector3 color = new Vector3(0, 0, 0);
                    for (int k = 0; k < raysPerPixel; k++)
                    {
                        float x = ((float)j) / 512 + ((float)r.Next(0, raysPerPixel) * pixelFraction);
                        float y = ((float)i) / 512 + ((float)r.Next(0, raysPerPixel) * pixelFraction);
                        Ray ray = camera.MakeRay(x, y);
                        color += Trace(ray, i == 265 && j % 16 == 0 && k == 0) / raysPerPixel;
                    }
                    *line++ = (uint)Color.FromArgb(
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
            Bitmap scaledImage = new Bitmap(image3D, new Size(screenSize, screenSize));

            Bitmap combined = new Bitmap(1024, 512);
            Graphics combinedGraphics = Graphics.FromImage(combined);
            combinedGraphics.DrawImage(scaledImage, 0, 0);
            combinedGraphics.DrawImage(debugger.image2D, 512, 0);

            return combined;
        }

        // This is where the magic happens! Trace a ray...
        Vector3 Trace(Ray ray, bool drawDebugLine)
        {
            // ... and see if it hits anything.
            Intersection intersect = scene.Intersect(ray);

            // Draw some debug output if it does hit anything.
            if (drawDebugLine)
            {
                if(intersect != null)
                debugger.DrawDebugLine(ray.origin.X, ray.origin.Z, intersect.intersectionPoint.X, intersect.intersectionPoint.Z, true);
                else
                debugger.DrawDebugLine(ray.origin.X, ray.origin.Z, ray.direction.X * 100, ray.direction.Z * 100, false);
            }

            Vector3 color = new Vector3(0, 0, 0);
            if (intersect == null)
            {
                return color;
            }

            if (intersect.primitive.material.isMirror)
            {
                Ray mirrorRay = new Ray() { origin = intersect.intersectionPoint };
                mirrorRay.direction = ray.direction - 2 * (intersect.normal)
                    * (Vector3.Dot(ray.direction, intersect.normal));
                mirrorRay.origin += 0.01f * mirrorRay.direction;
                mirrorRay.t = float.MaxValue;
                color += intersect.primitive.material.reflectiveness * Trace(mirrorRay, drawDebugLine);
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
