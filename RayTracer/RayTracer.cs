
//Raytracer, which owns the scene, camera and the display surface.The Raytracer implements a
//method Render, which uses the camera to loop over the pixels of the screen plane and to
//generate a ray for each pixel, which is then used to find the nearest intersection.The result is
//then visualized by plotting a pixel. For one line of pixels (typically line 256 for a 512x512
//window), it generates debug output by visualizing every Nth ray(where N is e.g. 10).

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace RayTracer
{
    public class RayTracer
    {
        static int screenSize = 512;
        Bitmap image3D = new Bitmap(512, 512);

        public Camera camera = new Camera();
        public Scene scene = new Scene();

        // maximum recursion depth
        short maxDepth = 3;

        // How many rays are cast per pixel; implementation of anti-aliasing.
        private int raysPerPixel = 5;

        BitmapData skyData;
        byte[] skyPixels;

        // trace a ray for each pixel and draw on the bitmap
        public unsafe Bitmap Render()
        {
            Debugger.SetupDebugView(camera, scene);
            
            // Get skybox data
            skyData = scene.sky.LockBits(new Rectangle(0, 0, scene.sky.Width, scene.sky.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            skyPixels = new byte[scene.sky.Width * scene.sky.Height * 3];
            Marshal.Copy(skyData.Scan0, skyPixels, 0, skyPixels.Length);

            // 3D image
            Rectangle rect = new Rectangle(0, 0, 512, 512);
            // lock the bits
            BitmapData bitmapData =
                image3D.LockBits(rect, ImageLockMode.WriteOnly,
                PixelFormat.Format32bppPArgb);

            Random r = new Random();
            // Get the address of the first line.
            uint* ptr = (uint*)bitmapData.Scan0;

            float pixelFraction = ((float)1 / 1024) / raysPerPixel;

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
                        color += Trace(ray, 0, i == 265 && j % 32 == 0 && k == 0) / raysPerPixel;
                    }
                    *line++ = (uint)Color.FromArgb(
                                        Helpers.ClampColor((int)(color.X * 255)),
                                        Helpers.ClampColor((int)(color.Y * 255)),
                                        Helpers.ClampColor((int)(color.Z * 255))
                                     ).ToArgb();
                }
                // Set pointer to next line
                ptr += bitmapData.Stride / 4;
            }

            // Unlock the bits.
            image3D.UnlockBits(bitmapData);
            Bitmap scaledImage = new Bitmap(image3D, new Size(screenSize, screenSize));

            // Unlock the skybox
            Marshal.Copy(skyPixels, 0, skyData.Scan0, skyPixels.Length);
            scene.sky.UnlockBits(skyData);

            Bitmap combined = new Bitmap(1024, 512);
            Graphics combinedGraphics = Graphics.FromImage(combined);
            combinedGraphics.DrawImage(image3D, 0, 0);
            combinedGraphics.DrawImage(Debugger.image2D, 512, 0);

            return combined;
        }

        // This is where the magic happens! Trace a ray...
        Vector3 Trace(Ray ray, short depth, bool drawDebugLine, bool absorb = false)
        {
            if (depth == maxDepth)
            {
                return new Vector3();
            }

            // see if the ray hits anything.
            Intersection intersect = scene.Intersect(ray);

            if (intersect == null)
            { // We didn't hit anything. return the background.
                return GetSkyColor(ray);
            }

            // Draw some debug output.
            if (drawDebugLine)
            {
                Debugger.DrawDebugLine(ray.origin.X, ray.origin.Z,
                    intersect.intersectionPoint.X,
                    intersect.intersectionPoint.Z, Color.Green);
            }

            // Magic!
            if (absorb && intersect.GetMaterial().absorbtion.Length() > 0)
            {
                return intersect.dist
                * -intersect.GetMaterial().absorbtion
                +  DoFancyColorCalculations(ray, intersect, depth, drawDebugLine);
            }
            else
                return DoFancyColorCalculations(ray, intersect, depth, drawDebugLine);
        }

        // Calculate the sky color and get it from a bitmap
        private Vector3 GetSkyColor(Ray ray)
        {
            if(ray.direction.X == 0 && ray.direction.Y == 0)
            {
                return new Vector3(0,0,0);
            }
            // Get location on screen
            float r = (float) ((1.0f / Math.PI) * Math.Acos(ray.direction.Z) / Math.Sqrt(ray.direction.X * ray.direction.X + ray.direction.Y * ray.direction.Y));
            int x = (int)(((ray.direction.X * r + 1f) / 2f) * scene.sky.Width);
            int y = scene.sky.Height - (int)(((ray.direction.Y * r + 1f) / 2f) * scene.sky.Height);

            int location = (y * scene.sky.Width + x) * 3;
            if(location > skyPixels.Length || location < 0)
            {
                return new Vector3();
            }

            return new Vector3(skyPixels[location + 2] / 256f, skyPixels[location + 1] / 256f, skyPixels[location] / 256f);
        }

        // Do we have to pass all these ugly arguments? ='( 
        // If we want to multi-thread it: yes.
        private Vector3 DoFancyColorCalculations(Ray ray, Intersection intersect, short depth, bool drawDebugLine)
        {
            depth++; // increment our recursion depth.
            Vector3 color = new Vector3();

            Material material = intersect.GetMaterial();
            if (material.isMirror)
            {
                color = material.reflectiveness * Reflect(ray, intersect, depth, drawDebugLine);
            }
            if (material.isDielectic)
            {
                float reflection = GetSchlickReflection(intersect, ray);

                // Now do the magic:
                color += material.transparency * reflection * Reflect(ray, intersect, depth, drawDebugLine); // reFLECT
                color += material.transparency * (1 - reflection) * Refract(ray, intersect, depth, drawDebugLine); //reFRACT
            }
            if (material.isShiny)
            {
                // Calculate reflection of shiny object
            }

            color += intersect.primitive.material.diffuseness
                * scene.DirectIllumination(intersect, drawDebugLine)
                * intersect.primitive.GetColor(intersect.intersectionPoint);
            return color;
        }

        private float GetSchlickReflection(Intersection intersect, Ray ray)
        {
            float nDotD = Vector3.Dot(intersect.normal, ray.direction);

            float eta1 = 1; // air
            float eta2 = intersect.GetMaterial().refractionIndex;

            if (nDotD < 0)
            {
                nDotD = -nDotD; // we are outside the surface, we want cos(theta) to be positive 
            }
            else
            {
                eta1 = eta2; eta2 = 1;
            }

            // Calculate some neccessary variables:
            float r0 = (eta1 - eta2) / (eta1 + eta2);
            r0 *= r0;
            float oneMinusCos = 1 - nDotD; // (1 - cos alpha)

            // Calculate how much light is reflected:
            return r0 + (1 - r0) * oneMinusCos * oneMinusCos * oneMinusCos * oneMinusCos * oneMinusCos;
        }

        private Vector3 Refract(Ray ray, Intersection intersect, short depht, bool drawDebugline)
        {
            float nDotD = -Vector3.Dot(intersect.normal, ray.direction);
            float eta1 = 1, eta2 = intersect.GetMaterial().refractionIndex;

            bool absorb = true;
            if (nDotD < 0)
            {
                // we are inside the surface, cos(theta) is already positive but reverse normal direction 
                eta1 = eta2; eta2 = 1;
                intersect.InvertNormal();
                absorb = false;
            }

            nDotD = -Vector3.Dot(intersect.normal, ray.direction);
            float eta = eta1 / eta2;
            float c = eta * eta * (1 - nDotD * nDotD);

            if (c > 1) return new Vector3(0, 0, 0); // Total internal reflection

            float cosT = (float)Math.Sqrt(1 - c);

            Ray refractRay = new Ray(intersect.intersectionPoint,
                eta * ray.direction + (eta * nDotD - cosT) * intersect.normal);

            refractRay.origin += 0.001f * refractRay.direction;  // offset by a small margin

            return Trace(refractRay, depht, drawDebugline, absorb);
        }

        private Vector3 Reflect(Ray ray, Intersection intersect, short depth, bool drawDebugLine)
        {
            Ray mirrorRay = new Ray()
            {
                origin = intersect.intersectionPoint,
                direction = ray.direction - 2 * (intersect.normal) * (Vector3.Dot(ray.direction, intersect.normal)),
                t = float.MaxValue
            };

            mirrorRay.origin += 0.001f * mirrorRay.direction; // offset by a small margin
            return Trace(mirrorRay, depth, drawDebugLine);
        }
    }
}
