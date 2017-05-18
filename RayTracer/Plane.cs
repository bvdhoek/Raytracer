using System;
using System.Numerics;

namespace RayTracer 
{
    class Plane : Primitive
    {
        // normal of the plane
        private Vector3 normal;

        public Plane(Vector3 normal, Vector3 origin) : base(origin)
        {
            this.normal = normal;
        }

        public Plane(Vector3 normal, Vector3 origin, Vector3 color) : base(origin, color)
        {
            this.normal = normal;
        }

        public override Intersection Intersect(Ray ray)
        {
            float denom = Vector3.Dot(normal, ray.direction);
            Vector3 rayToPlane = origin - ray.origin;
            float t = Vector3.Dot(rayToPlane, normal) / denom;
            if (t >= 0)
            {
                if (t < ray.t) ray.t = t;
                return new Intersection(this, normal, ray.t * ray.direction, ray.t);
            }
            return null;
        }
    }
}
