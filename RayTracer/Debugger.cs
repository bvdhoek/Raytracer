using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class Debugger
    {
        public Bitmap image2D = new Bitmap(512, 512);

        float scale;
        Graphics graphics2D;

        // Draw a debug ray. If it intersected, make it red. Otherwise, make it yellow.
        public void DrawDebugLine(float x1, float z1, float x2, float z2, bool intersect)
        {
            graphics2D.DrawLine(intersect ? new Pen(Color.Red) : new Pen(Color.Yellow),
                x1 * scale + image2D.Width / 2,
                image2D.Height - z1 * scale,
                x2 * scale + image2D.Width / 2,
                image2D.Height - z2 * scale);
        }

        internal void SetupDebugView(Camera camera, Scene scene)
        {

            graphics2D = Graphics.FromImage(image2D);
            SetDebugScale(camera, scene);
            DrawDebugPrimitives(scene);
            DrawDebugScreen(camera);

        }
        // Draw a white line in de debugview representing the screen.
        private void DrawDebugScreen(Camera camera)
        {
            Vector3 p0 = camera.p0;
            Vector3 p1 = camera.p1;
            float widthOffset = image2D.Width / 2 - p1.X - p0.X;

            graphics2D.DrawLine(new Pen(Color.White),
                p0.X * scale + widthOffset,
                image2D.Height - p0.Z * scale,
                p1.X * scale + widthOffset,
                image2D.Height - p1.Z * scale);
        }

        // Determine furthest object from the camera for scale of the debugview
        private void SetDebugScale(Camera camera, Scene scene)
        {
            float maxX = 0f;
            float maxZ = 0f;
            foreach (Primitive primitive in scene.primitives)
            {
                if (primitive.GetType() == typeof(Sphere))
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
            }

            float max = maxX > maxZ ? maxX : maxZ + 2;
            scale = image2D.Width / max;
        }

        private void DrawDebugPrimitives(Scene scene)
        {
            foreach (Primitive primitive in scene.primitives)
            {
                // Draw a nice ellipse for each sphere
                if (primitive.GetType() == typeof(Sphere))
                {
                    Sphere sphere = (Sphere)primitive;
                    Color color = Color.FromArgb(
                        (int)primitive.material.color.X * 255,
                        (int)primitive.material.color.Y * 255,
                        (int)primitive.material.color.Z * 255
                        );

                    graphics2D.DrawEllipse(new Pen(color),
                        (sphere.origin.X - sphere.r) * scale + image2D.Width / 2, // center drawing
                        image2D.Height - (sphere.origin.Z + sphere.r) * scale,
                        2 * sphere.r * scale,
                        2 * sphere.r * scale);
                }
            }
        }
    }
}
