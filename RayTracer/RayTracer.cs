
//Raytracer, which owns the scene, camera and the display surface.The Raytracer implements a
//method Render, which uses the camera to loop over the pixels of the screen plane and to
//generate a ray for each pixel, which is then used to find the nearest intersection.The result is
//then visualized by plotting a pixel. For one line of pixels (typically line 256 for a 512x512
//window), it generates debug output by visualizing every Nth ray(where N is e.g. 10).

using System.Drawing;
using System.Numerics;

namespace RayTracer
{
    public class RayTracer
    {
        Bitmap image3D = new Bitmap(512, 512);

        Camera camera = new Camera();
        Scene scene = new Scene();

        // Make a ray for each pixel and put them on the bitmap image.
        public Bitmap Render()
        {
            for (int i = 0; i < 512; i++)
            {
                for (int j = 0; j < 512; j++)
                {
                    float x = ((float)i) / 512;
                    float y = ((float)j) / 512;
                    Ray ray = camera.MakeRay(x, y);
                    PlotPixel(image3D, Trace(ray), i, j);
                }
            }
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
            {
                //TODO: check why this doesn't work when there's a debug view.
                Ray mirrorRay = new Ray();
                mirrorRay.origin = intersect.intersectionPoint;
                mirrorRay.direction = Vector3.Normalize((ray.direction * ray.t) - 2 * Vector3.Dot((ray.direction * ray.t), intersect.normal) * intersect.normal);
                mirrorRay.origin += mirrorRay.direction * 0.001f;
                return Trace(mirrorRay);
                //   return intersect.primitive.material.color * Trace( );
            } 
            return scene.DirectIllumination(intersect) * intersect.primitive.material.color;
        }


        private void PlotPixel(Bitmap bitmap, Vector3 color, int i, int j)
        {
            // Plot color to the bitmap using the coördinates
            bitmap.SetPixel(i, j, Color.FromArgb(
                Clamp((int)(color.X * 255)),
                Clamp((int)(color.Y * 255)),
                Clamp((int)(color.Z * 255))));
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
