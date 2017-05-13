using System.Numerics;

namespace RayTracer
{
    abstract class Primitive
    {
        public Vector3 o;
        public Material material { get; private set; } = new Material();

        public Primitive()
        {
            // default color is red.
            material.SetColor(new Vector3(1, 0, 0));
        }

        public Primitive(Vector3 color)
        {
            material.SetColor(color);
        }

        public abstract Intersection Intersect(Ray ray);

        // Not implemented yet.
        // TODO: Implement when materials are implemented
        public bool IsMirror()
        {
            return false;
        }
    }
}
