using System;
using System.Numerics;

namespace RayTracer {
    //    Scene, which stores a list of primitives and light sources.It implements a scene-level Intersect
    //method, which loops over the primitives and returns the closest intersection
    public class Scene
    {
        public Primitive[] primitives = new Primitive[4];
        private Light[] lights = new Light[1];

        // Create new scene and populate with some default lights and primitives.
        public Scene()
        {
            this.lights[0] = new Light();

            Vector3 spherePos = new Vector3(0, 0, 7);
            this.primitives[0] = new Sphere(spherePos, 1f, new Vector3(1, 0, 1), 0.2f, 0.8f);

            spherePos.Z += 2;
            spherePos.X -= 1;
            this.primitives[1] = new Sphere(spherePos, 0.5f, new Vector3(0, 1, 0));

            spherePos.X += 4;
            this.primitives[2] = new Sphere(spherePos, 0.5f, new Vector3(0, 0, 1));

            primitives[3] = new Plane(new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0, 1, 1));
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
        internal Vector3 DirectIllumination(Intersection intersect)
        {
            Vector3 color = new Vector3();

            for (int i = 0; i < lights.Length; i++)
            {
                color += CalculateIllumination(intersect, i);
            }
            return color;
        }

        // Get illumination for lightsource with index i
        private Vector3 CalculateIllumination(Intersection intersect, int i)
        {
            Ray shadowRay = new Ray();
            shadowRay.origin = intersect.intersectionPoint;

            // Keep an non-normalized directon in case we need to calculate length later
            Vector3 rayDirection = lights[i].pos - intersect.intersectionPoint;

            // Normalize for calulating intersections
            shadowRay.direction = Vector3.Normalize(rayDirection);

            // Offset the origin by a small margin
            shadowRay.origin += 0.001f * shadowRay.direction;

            //  Check if any primitives intersect with this shadowray
            if (Intersect(shadowRay) != null)
            {
                // Intersection point is in the shadow, return black.
                return new Vector3(0, 0, 0);
            }

            // No intersection happened so we can calculate light color/intensity:
            float dist = rayDirection.Length();
            float attenuation = 1 / (dist * dist);
            return lights[0].color * Vector3.Dot(intersect.normal, shadowRay.direction) * attenuation;
        }
    }
}
