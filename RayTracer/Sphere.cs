using System.Numerics;
using System;

namespace RayTracer
{
    class Sphere : Primitive
    {
        // Radius
        public float r;

        public Sphere(Vector3 origin, float r) : base(origin)
        {
            this.r = r;
        }

        public Sphere(Vector3 origin, 
            float r, 
            Vector3 color,
            float reflectiveness = 0, 
            float transparancy = 0, 
            float shine = 0) : base(origin, color, reflectiveness, transparancy, shine)
        {
            this.r = r;
        }

        // Check if the ray intersects this sphere. If yes, return an intersection. Otherwise return null.
        override public Intersection Intersect(Ray ray)
        {
            Vector3 c = origin - ray.origin;
            float t = Vector3.Dot(c, ray.direction);
            Vector3 q = c - t * ray.direction;
            float p2 = Vector3.Dot(q, q);

            if (p2 > r * r)
            { // We didn't hit the sphere.
                return null;
            }

            t -= (float)Math.Sqrt(r * r - p2);

            if ((t < ray.t) && (t > 0))
            { // Set length of the ray to t.
                ray.t = t;
            }
            else return null;
            // return a new intersect with: this, the normal to the sphere, the intersection point, the distance
            return new Intersection(this,
                Vector3.Normalize((ray.origin + ray.t * ray.direction) - origin),
                ray.origin + ray.direction * ray.t,
                ray.t);
        }
    }
}
