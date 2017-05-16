using System;
using System.Numerics;

namespace RayTracer
{
    //    Scene, which stores a list of primitives and light sources.It implements a scene-level Intersect
    //method, which loops over the primitives and returns the closest intersection
    class Scene
    {
        private Primitive[] primitives = new Primitive[1];
        private Light[] lights = new Light[1];

        // Create new scene and populate with some default lights and primitives.
        public Scene()
        {
            this.lights[0] = new Light();

            Vector3 spherePos = new Vector3(0, 0, 100);
            this.primitives[0] = new Sphere(spherePos, 3);
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

        internal Vector3 DirectIllumination(Intersection intersect)
        {
            return intersect.primitive.material.color;
            Ray shadowRay = new Ray();
            shadowRay.origin = intersect.intersectionPoint;
            
            // Keep an non-normalized directon in case we need to calculate length later
            Vector3 rayDirection = lights[0].pos - intersect.intersectionPoint;

            // Normalize for calulating intersections
            shadowRay.direction = Vector3.Normalize(rayDirection);

            for (int i = 0; i < primitives.Length; i++)
            {
                if (Intersect(shadowRay) != null)
                {
                    // Intersection point is in the shadow, return black.
                    return new Vector3(0, 0, 0);
                }
            }
            // No intersection happened so we can calculate light color/intensity:
            float dist = rayDirection.Length();
            float attenuation = 1 / (dist * dist);
            return lights[0].color * Vector3.Dot(intersect.normal, shadowRay.direction) * attenuation;
        }
    }
}
