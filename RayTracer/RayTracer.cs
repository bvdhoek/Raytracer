
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

        // maximum recursion depth
        short maxDepth = 4;

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
                    Vector3 color = Trace(ray, 0, i == 256 && j % 16 ==0);
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
        Vector3 Trace(Ray ray, short depth, bool drawDebugLine)
        {
            // Background color. This will be the color of the skydome once that's implemented
            Vector3 color = new Vector3(0, 0, 0);

            if (depth == maxDepth)
            {
                return color;
            }

            // see if the ray hits anything.
            Intersection intersect = scene.Intersect(ray);

            // Draw some debug output.
            if (drawDebugLine)
            {
                if(intersect != null)
                debugger.DrawDebugLine(ray.origin.X, ray.origin.Z, intersect.intersectionPoint.X, intersect.intersectionPoint.Z, true);
                else
                debugger.DrawDebugLine(ray.origin.X, ray.origin.Z, ray.direction.X * 100, ray.direction.Z * 100, false);
            }

            if (intersect == null)
            {
                return color;
            }
            depth++; // increment our recursion depth.
            Material material = intersect.GetMaterial();

            if (material.isMirror)
            {
                color += material.reflectiveness * Trace(Reflect(ray, intersect), depth, drawDebugLine);
            }
            if (material.isDielectic)
            {
                // Calculate some neccessary variables:
                float r0Root = (1 - material.refractionIndex) / (1 + material.refractionIndex);
                float r0 = r0Root * r0Root; // r0
                float oneMinusCos = 1 - Vector3.Dot(intersect.normal, ray.direction); // (1 - cos alpha)

                // Calculate how much light is reflected:
                float reflection = r0 + (1 - r0) * oneMinusCos * oneMinusCos * oneMinusCos * oneMinusCos * oneMinusCos;
                // ... and how much light is refracted
                float refraction = 1 - reflection;

                // Now do the magic:
                color += material.transparency * reflection * Trace(Reflect(ray, intersect), depth, false); // reFLECT
                color += material.transparency * refraction * Trace(Refract(ray, intersect), depth, drawDebugLine); //reFRACT
            }
            if(material.isShiny)
            {
                // Calculate reflection of shiny object
            }
            color += material.diffuseness * scene.DirectIllumination(intersect) * material.color;
            return color;
        }

        private Ray Refract(Ray ray, Intersection intersect)
        {
            float c1 = Vector3.Dot(intersect.normal, ray.direction);
            float c2 = (float)Math.Sqrt(1 - (intersect.primitive.material.e * intersect.primitive.material.e) * (1 - (c1 * c1)));
            Ray refractRay = new Ray()
            {
                direction = (intersect.primitive.material.e * ray.direction) 
                    + ((intersect.primitive.material.e * (c1 - c2)) * intersect.normal),
                origin = intersect.intersectionPoint,
                t = float.MaxValue
            };
            refractRay.origin += 0.001f * refractRay.direction;
            return refractRay;
        }

        private Ray Reflect(Ray ray, Intersection intersect)
        {
            Ray mirrorRay = new Ray()
            {
                origin = intersect.intersectionPoint,
                direction = ray.direction - 2 * (intersect.normal) * (Vector3.Dot(ray.direction, intersect.normal)),
                t = float.MaxValue
            };

            mirrorRay.origin += 0.01f * mirrorRay.direction; // offset by a small margin
            return mirrorRay;
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
