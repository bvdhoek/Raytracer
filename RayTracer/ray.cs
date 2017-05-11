using System.Numerics;

namespace RayTracer
{
    struct Ray
    {
        public Vector3 d; // ray direction
        public Vector3 o; // ray origin
        public float t; // ray length

        public Ray(Vector3 origin, Vector3 direction, float length)
        {
            d = Vector3.Normalize(direction);
            o = origin;
            t = length;
        }
    }
}
