using RayTracer.Properties;
using System;
using System.Drawing;
using System.Numerics;

namespace RayTracer {
    //    Scene, which stores a list of primitives and light sources.It implements a scene-level Intersect
    //method, which loops over the primitives and returns the closest intersection
    public class Scene
    {
        public Primitive[] primitives = new Primitive[6];
        private Light[] lights = new Light[1];
        public Bitmap sky;
       
        // Create new scene and populate with some default lights and primitives.
        public Scene()
        {
            sky = Resources.uffizi;

            this.lights[0] = new Light();
            Vector3 spherePos = new Vector3(-2f, 0, 7);
            this.primitives[0] = new Sphere(spherePos, 1f, new Vector3(1, 0, 0),0 , 0.9f);
            this.primitives[0].material.absorbtion = new Vector3(0.1f, 0, 0);

            spherePos = new Vector3(-1.3f, 0f, 13);
            this.primitives[1] = new Sphere(spherePos, 1f, new Vector3(0, 1, 0), 0.1f);

            spherePos = new Vector3(1.5f, -0.3f, 7);
            this.primitives[2] = new Sphere(spherePos, 0.7f, new Vector3(0, 0.2f, 0.6f), 0.6f);

            spherePos = new Vector3(0.8f, -0.7f, 5.5f);
            this.primitives[3] = new Sphere(spherePos, 0.3f, new Vector3(0f, 0.4f, 0.7f), 0.3f);

            spherePos = new Vector3(-0.7f, -0.75f, 5);
            this.primitives[4] = new Sphere(spherePos, 0.25f, new Vector3(1, 1, 0), 0.25f);

            primitives[5] = new Plane(new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0, 1, 1), 0.35f)
            {
                isCheckered = true
            };
        }

        // Find intersection of the ray with nearest object in the scene.
        public Intersection Intersect(Ray ray)
        {
            Intersection nearest = null;
            Intersection next = null;

            for (int i = 0; i < primitives.Length; i++)
            {
                next = primitives[i].Intersect(ray);
                if (next != null && (nearest == null || next.dist < nearest.dist))
                {
                    nearest = next;
                }
            }
            return nearest;
        }

        // Get the illumination for all light sources
        internal Vector3 DirectIllumination(Intersection intersect, bool drawDebugLine)
        {
            Vector3 color = new Vector3();

            for (int i = 0; i < lights.Length; i++)
            {
                color += CalculateIllumination(intersect, i, drawDebugLine);
            }
            return color;
        }

        // Get illumination for lightsource with index i
        private Vector3 CalculateIllumination(Intersection intersect, int i, bool drawDebugLine)
        {
            Ray shadowRay = new Ray();
            shadowRay.origin = intersect.intersectionPoint;

            // Keep an non-normalized directon in case we need to calculate length later
            Vector3 rayDirection = lights[i].pos - intersect.intersectionPoint;
            shadowRay.t = rayDirection.Length();

            // Normalize for calulating intersections
            shadowRay.direction = Vector3.Normalize(rayDirection);

            // Offset the origin by a small margin
            shadowRay.origin += 0.001f * shadowRay.direction;

            //  Check if any primitives intersect with this shadowray
            Intersection lightBlocker = Intersect(shadowRay);
            if (lightBlocker != null)
            {
                // Intersection point is in the shadow, return black.
                if (drawDebugLine)
                    Debugger.DrawDebugLine(
                        shadowRay.origin.X,
                        shadowRay.origin.Z,
                        lightBlocker.intersectionPoint.X,
                        lightBlocker.intersectionPoint.Z,
                        System.Drawing.Color.Orange);

                return new Vector3(0, 0, 0);
            }

            //shadowRay = new Ray();
            //shadowRay.origin = intersect.intersectionPoint;

            //// Keep an non-normalized directon in case we need to calculate length later
            //rayDirection = lights[i].pos - intersect.intersectionPoint;
            //shadowRay.t = rayDirection.Length();

            //// Normalize for calulating intersections
            //shadowRay.direction = Vector3.Normalize(rayDirection);

            //// Offset the origin by a small margin
            //shadowRay.origin += 0.001f * shadowRay.direction;

            ////  Check if any primitives intersect with this shadowray
            //lightBlocker = Intersect(shadowRay);


            // Intersection point is in the shadow, return black.
            if (drawDebugLine)
                Debugger.DrawDebugLine(
                    shadowRay.origin.X,
                    shadowRay.origin.Z,
                    lights[i].pos.X,
                    lights[i].pos.Z,
                    System.Drawing.Color.Yellow);

            // No intersection happened so we can calculate light color/intensity:
            float dist = rayDirection.Length();
            float attenuation = 1 / (dist * dist);
            return lights[0].color * Vector3.Dot(intersect.normal, shadowRay.direction) * attenuation;
        }
    }
}
