
//Raytracer, which owns the scene, camera and the display surface.The Raytracer implements a
//method Render, which uses the camera to loop over the pixels of the screen plane and to
//generate a ray for each pixel, which is then used to find the nearest intersection.The result is
//then visualized by plotting a pixel. For one line of pixels (typically line 256 for a 512x512
//window), it generates debug output by visualizing every Nth ray(where N is e.g. 10).

using System;

namespace RayTracer
{
    class RayTracer
    {
        Camera camera;
        Scene scene;

        void Render()
        {
            for (int i = 0; i < 512; i++)
            {
                for (int j = 0; j < 512; j++)
                {
                    float x = ((float)i) / 512;
                    float y = ((float)i) / 512;
                    Ray ray = camera.MakeRay(x, y);
                    PlotPixel(scene.Intersect(ray), i, j);
                }
            }
        }

        private void PlotPixel(Intersection intersection, int i, int j)
        {
            throw new NotImplementedException();
        }
    }
}
