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
            p0 = screenCenter + new Vector3(-1, -1, 0);
            p1 = screenCenter + new Vector3(1, -1, 0);
            p2 = screenCenter + new Vector3(-1, 1, 0);
        }

        // Make a new ray from relative screen coördinates. 0 <= x, y <= 1
        // Returns: A normalized ray from camera position through coordinate (x, y) on the screen
        internal Ray MakeRay(float x, float y)
        {
            Vector3 screenLocation = p0 + x * (p1 - p0) + y * (p2 - p0);
            Vector3 direction = screenLocation - this.pos;
            return new Ray(pos, direction, 1);
        }
    }
}
