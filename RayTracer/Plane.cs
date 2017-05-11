using System.Numerics;

namespace RayTracer 
{
    class Plane : Primitive
    {
        // normal of the plane
        private Vector3 normal;

        // distance from origin
        private Vector3 d;

        public Plane(Vector3 normal, Vector3 d)
        {
            this.normal = normal;
            this.d = d;
        }

        public Plane(Vector3 normal, Vector3 d, Vector3 color) : base(color)
        {
            this.normal = normal;
            this.d = d;
        }
    }
}
