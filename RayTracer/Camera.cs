using System.Numerics;

namespace RayTracer
{
    public class Camera
    {
        // Camera position
        public Vector3 pos = new Vector3(0, 0, 0);

        // Camera direction
        public Vector3 d = new Vector3(0, 0, 1);
        public float distanceToScreen = 2;

        Vector3 screenCenter, up, right;

        // Screen corners:
        public Vector3 p0, p1, p2;

        public Camera()
        {
            up = new Vector3(0, 1, 0);
            setDirections();
            setScreen();
        }

        private void setDirections()
        {
            right = Vector3.Cross(up, d);
            up = Vector3.Cross(right, d);
        }

        private void setScreen()
        {
            screenCenter = pos + distanceToScreen * d;
            p0 = screenCenter + new Vector3(-1, 1, 0);
            p1 = screenCenter + new Vector3(1, 1, 0);
            p2 = screenCenter + new Vector3(-1, -1, 0);
        }

        public void Zoom(float scalar)
        {
            distanceToScreen *= scalar;
            setScreen();
            Debugger.Reset();
        }

        public void MoveRight()
        {
            pos += 0.1f * right;
            setScreen();
        }

        public void MoveLeft()
        {
            pos -= 0.1f * right;
            setScreen();
        }

        public void MoveUp()
        {
            pos += 0.1f * up;
            setScreen();
        }

        public void MoveDown()
        {
            pos -= 0.1f * up;
            setScreen();
        }

        public void MoveForward()
        {
            pos += 0.1f * d;
            setScreen();
        }

        public void MoveBack()
        {
            pos -= 0.1f * d;
            setScreen();
        }

        public void TurnRight(float degrees)
        {

        }

        public void TurnLeft(float degrees)
        {

        }

        public void TurnUp(float degrees)
        {

        }

        public void TurnDown(float degrees)
        {

        }

        // Make a new ray from relative screen coördinates. 0 <= x, y <= 1
        // Returns: A normalized ray from camera position through coordinate (x, y) on the screen
        internal Ray MakeRay(float x, float y)
        {
            Vector3 screenLocation = p0 + x * (p1 - p0) + y * (p2 - p0);
            Vector3 direction = screenLocation - this.pos;
            return new Ray(pos, direction);
        }
    }
}
