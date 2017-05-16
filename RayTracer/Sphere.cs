﻿using System.Numerics;
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

        override public Intersection Intersect(Ray ray)
        {
            Vector3 c = pos - ray.o;
            float t = Vector3.Dot(c, ray.d);
            Vector3 q = c - t * ray.d;
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
            // return a new intersect with: this, the normal to the sphere, the intersection point, the distance
            return new Intersection(this, Vector3.Normalize(ray.t * ray.d - o), ray.d * ray.t, ray.t);
        }
    }
}