using System.Numerics;

namespace RayTracer
{
    class Light
    {
        // default position
        public Vector3 pos = new Vector3(-1, 8 , 5);
        // default color
        public Vector3 color = new Vector3(70, 70, 70);

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
