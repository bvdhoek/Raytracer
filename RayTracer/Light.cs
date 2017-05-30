using System.Numerics;

namespace RayTracer
{
    class Light
    {
        // default position
        public Vector3 pos = new Vector3(0, 4, 0);
        // default color
        public Vector3 color = new Vector3(40, 40, 40);

        public Light() { }

        public Light(Vector3 position)
        {
            this.pos = position;
        }

        public Light(Vector3 position, Vector3 color)
        {
            this.pos = position;
            this.color = color;
        }
    }
}
