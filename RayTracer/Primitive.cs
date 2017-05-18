using System.Numerics;

namespace RayTracer
{
    abstract class Primitive
    {
        public Vector3 origin;
        public Material material { get; private set; } = new Material();

        public Primitive(Vector3 origin)
        {
            this.origin = origin;
            // default color is red.
            material.SetColor(new Vector3(1, 0, 0));
        }

        public Primitive(Vector3 origin, Vector3 color, float reflectiveness = 0, float transparancy = 0, float shine = 0)
        {
            this.origin = origin;
            material.SetColor(color, reflectiveness, transparancy, shine);
        }

        public abstract Intersection Intersect(Ray ray);

        // Not implemented yet.
        // TODO: Implement when materials are implemented
        public bool IsMirror()
        {
            return material.isMirror;
        }
    }
}
