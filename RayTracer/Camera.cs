using System;
using System.Numerics;

namespace RayTracer
{
    class Camera
    {
        // Camera position
        Vector3 pos = new Vector3(0, 0, 0);

        // Camera direction
        Vector3 d = new Vector3(0, 0, 1);

        Vector3 screenCenter;

        // Screen corners:
        Vector3 p0, p1, p2;
        public Camera()
        {
            this.screenCenter = pos + 10 * d;
            p0 = pos + new Vector3(-1, -1,0);
            p1 = pos + new Vector3(1, -1, 0);
            p2 = pos + new Vector3(-1, 1, 0);
        }

        internal Ray MakeRay(float x, float y)
        {
            Vector3 screenLocation = p0 + x * (p1 - p0) + y * (p2 - p0);
            Vector3 direction = Vector3.Normalize(screenLocation - this.pos);            return new Ray(pos, direction, 1);
        }
}
}
