using System.Numerics;

namespace RayTracer
{
    class Intersection
    {
        // Primitive we intersected
        Primitive primitive;
        // normal vector at intersection point
        Vector3 normal;
        // distance from origin to intersection
        public float dist;
    }
}
