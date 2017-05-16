
//Raytracer, which owns the scene, camera and the display surface.The Raytracer implements a
//method Render, which uses the camera to loop over the pixels of the screen plane and to
//generate a ray for each pixel, which is then used to find the nearest intersection.The result is
//then visualized by plotting a pixel. For one line of pixels (typically line 256 for a 512x512
//window), it generates debug output by visualizing every Nth ray(where N is e.g. 10).

using System;
using System.Drawing;
using System.Numerics;

namespace RayTracer
{
   public class RayTracer
    {
        Bitmap image3D = new Bitmap(512, 512);

        Camera camera = new Camera();
        Scene scene = new Scene();

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
            if(intersect == null)
            {
                return new Vector3(0, 0, 0);
            }
            if (intersect.primitive.material.isMirror)
            { } // Not implemented yet; cast a mirror ray:
            //   return intersect.primitive.material.color * Trace( );
            return scene.DirectIllumination(intersect) * intersect.primitive.material.color;
        }


        private void PlotPixel(Bitmap bitmap, Vector3 color, int i, int j)
        {
            // Plot color to the bitmap using the coördinates
            bitmap.SetPixel(i, j, Color.FromArgb(255, (int)color.X * 255, (int)color.Y * 255, (int)color.Z * 255));
        }
    }
}
