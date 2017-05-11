using System.Numerics;

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
    }
}
