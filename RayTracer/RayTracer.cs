
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

        public Camera camera = new Camera();
        public Scene scene = new Scene();

        // maximum recursion depth
        short maxDepth = 4;

        // How many rays are cast per pixel; implementation of anti-aliasing.
        private int raysPerPixel = 2;

        // trace a ray for each pixel and draw on the bitmap
        public unsafe Bitmap Render()
        {
            Debugger.SetupDebugView(camera, scene);

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

            Bitmap combined = new Bitmap(1024, 512);
            Graphics combinedGraphics = Graphics.FromImage(combined);
            combinedGraphics.DrawImage(image3D, 0, 0);
            combinedGraphics.DrawImage(Debugger.image2D, 512, 0);

            return combined;
        }

        // This is where the magic happens! Trace a ray...
        Vector3 Trace(Ray ray, short depth, bool drawDebugLine, bool absorb = false)
        {
            // Background color. This will be the color of the skydome once that's implemented
            Vector3 backgroundColor = new Vector3(0, 0, 0);

            if (depth == maxDepth)
            {
                return backgroundColor;
            }

            // see if the ray hits anything.
            Intersection intersect = scene.Intersect(ray);

            // Draw some debug output.
            if (drawDebugLine)
            {
                if (intersect == null)
                    Debugger.DrawDebugLine(ray.origin.X, ray.origin.Z, ray.origin.X + ray.direction.X * 100, ray.origin.Z + ray.direction.Z * 100, Color.Red);
                else
                    Debugger.DrawDebugLine(ray.origin.X, ray.origin.Z, 
                        intersect.intersectionPoint.X, 
                        intersect.intersectionPoint.Z, Color.Green);
            }

            if (intersect == null)
            {
                return backgroundColor;
            }

            // Magic!
            if (absorb && intersect.GetMaterial().absorbtion.Length() > 0)
            {
                return intersect.dist
                * -intersect.GetMaterial().absorbtion
                +  DoFancyColorCalculations(ray, intersect, backgroundColor, depth, drawDebugLine);
            }
            else
                return DoFancyColorCalculations(ray, intersect, backgroundColor, depth, drawDebugLine);
        }

        // Do we have to pass all these ugly arguments? ='( 
        // If we want to multi-thread it: yes.
        private Vector3 DoFancyColorCalculations(Ray ray, Intersection intersect, Vector3 color, short depth, bool drawDebugLine)
        {
            depth++; // increment our recursion depth.
            Material material = intersect.GetMaterial();
            if (material.isMirror)
            {
                color += material.reflectiveness * Reflect(ray, intersect, depth, drawDebugLine);
            }
            if (material.isDielectic)
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
                    eta1 = eta2;
                    eta2 = 1;
                } 

                // Calculate some neccessary variables:
                float r0 = (eta1 - eta2) / (eta1 + eta2);
                r0 *= r0;
                float oneMinusCos = 1 - nDotD; // (1 - cos alpha)

                // Calculate how much light is reflected:
                float reflection = r0 + (1 - r0) * oneMinusCos * oneMinusCos * oneMinusCos * oneMinusCos * oneMinusCos;
                // ... and how much light is refracted
                float refraction = 1 - reflection;

                // Now do the magic:
                color += material.transparency * reflection * Reflect(ray, intersect, depth, drawDebugLine); // reFLECT
                color += material.transparency * refraction * Refract(ray, intersect, depth, drawDebugLine, eta1, eta2); //reFRACT
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

        private Vector3 Refract(Ray ray, Intersection intersect, short depht, bool drawDebugline, float eta1, float eta2)
        {
            float nDotD = Vector3.Dot(intersect.normal, ray.direction);
            bool absorb = false;
            if (nDotD < 0)
            {
                nDotD = -nDotD; // we are outside the surface, we want cos(theta) to be positive
                absorb = true;
            }
            else
            {
                intersect.InvertNormal(); // we are inside the surface, cos(theta) is already positive but reverse normal direction 
            }

            float e = eta1 / eta2;
            float c = 1 - e * e * (1 - nDotD * nDotD);

            if (c < 0) return new Vector3(0, 0, 0);

            Ray refractRay = new Ray(intersect.intersectionPoint,
                (e * ray.direction) + ((e * (nDotD - (float)Math.Sqrt(c)) * intersect.normal)));

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
