using System.Numerics;

namespace RayTracer
{
    struct Ray
    {
        Vector3 d; // ray direction
        Vector3 o; // ray origin
        float t; // ray length

        public Ray(Vector3 origin, Vector3 direction, float length)
        {
            d = Vector3.Normalize(direction);
            o = origin;
            t = length;
        }
    }
}
