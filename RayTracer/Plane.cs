using System;
using System.Numerics;

namespace RayTracer 
{
    class Plane : Primitive
    {
        // normal of the plane
        private Vector3 normal;

        // distance from origin
        private Vector3 dist;

        public Plane(Vector3 normal, Vector3 d)
        {
            this.normal = normal;
            this.dist = d;
        }

        public Plane(Vector3 normal, Vector3 d, Vector3 color) : base(color)
        {
            this.normal = normal;
            this.dist = d;
        }

        public override Intersection Intersect(Ray ray)
        {
            throw new NotImplementedException();
        }
    }
}
