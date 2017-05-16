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
            throw new NotImplementedException();
        }
    }
}
