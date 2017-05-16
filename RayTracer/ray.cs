using System.Numerics;

namespace RayTracer
{
    struct Ray
    {
        public Vector3 direction; // ray direction
        public Vector3 origin; // ray origin
        public float t; // ray length

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.direction = Vector3.Normalize(direction);
            this.origin = origin;
            this.t = float.MaxValue;
        }
    }
}
