using System.Numerics;

namespace RayTracer
{
    abstract class Primitive
    {
        private Vector3 color;

        public Primitive()
        {
            // default color is red.
            this.color = new Vector3(1, 0, 0);
        }

        public Primitive(Vector3 color)
        {
            this.color = color;
        }
    }
}
