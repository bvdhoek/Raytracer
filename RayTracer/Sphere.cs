using System.Numerics;
using System;

namespace RayTracer 
{
    class Sphere : Primitive
    {
        // Position
        private Vector3 pos;

        // Radius
        private float r;

        public Sphere(Vector3 pos, float r)
        {
            this.pos = pos;
            this.r = r;
        }

        public Sphere(Vector3 pos, float r, Vector3 color) : base(color)
        {
            this.pos = pos;
            this.r = r;
        }

        public float R2()
        {
            return r * r;
        }

        override
        public Intersection Intersect(Ray ray)
        {
            Vector3 c = pos - ray.o;
            float t = Vector3.Dot(c, ray.d);
            Vector3 q = c - t * ray.d;
            float p2 = Vector3.Dot(q, q);
            if (p2 > r * r) return null;
            t -= (float) Math.Sqrt(R2() - p2);
            if ((t < ray.t) && (t > 0)) ray.t = t;
            return new Intersection(this, Vector3.Normalize(ray.t * ray.d - o), ray.t);
        }
    }
}
