using System.Numerics;

namespace RayTracer
{
    class Intersection
    {
        // Primitive we intersected
        Primitive primitive;
        // normal vector at intersection point
        Vector3 normal;
        // distance from origin of the ray to intersection
        public float dist;

        public Intersection(Primitive p, Vector3 n, float d)
        {
            primitive = p;
            normal = n;
            dist = d;
        }
    }
}
