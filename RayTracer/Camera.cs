using System;
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

        public Vector3 screenCenter, up, right;

        // Screen corners:
        public Vector3 p0, p1, p2;

        public Camera()
        {
            up = new Vector3(0, 1, 0);
            right = Vector3.Cross(up, d);
            up = Vector3.Cross(d, right);
            setScreen();
        }

        private void setScreen()
        {
            screenCenter = pos + distanceToScreen * d;
            p0 = screenCenter + up - right;
            p1 = screenCenter + up + right;
            p2 = screenCenter - up - right;
        }

        public void Zoom(float scalar)
        {
            distanceToScreen *= scalar;
            setScreen();
        }

        public void Move(float distance, Vector3 directionVector)
        {
            pos += distance * directionVector;
            setScreen();
        }

        // rotation methods assume degrees < 90
        public void RotateRight(float degrees)
        {
            d = Vector3.Normalize(d + ((float) Math.Tan(degrees / 180 * Math.PI)) * right);
            right = Vector3.Cross(up, d);
            setScreen();
        }

        public void RotateLeft(float degrees)
        {
            d = Vector3.Normalize(d + ((float)-Math.Tan(degrees / 180 * Math.PI)) * right);
            right = Vector3.Cross(up, d);
            setScreen();
        }

        public void RotateUp(float degrees)
        {
            d = Vector3.Normalize(d + ((float)Math.Tan(degrees / 180 * Math.PI)) * up);
            up = Vector3.Cross(d, right);
            setScreen();
        }

        public void RotateDown(float degrees)
        {
            d = Vector3.Normalize(d + ((float)-Math.Tan(degrees / 180 * Math.PI)) * up );
            up = Vector3.Cross(d, right);
            setScreen();
        }

        public void TurnRight(float degrees)
        {
            up = Vector3.Normalize(up + ((float)Math.Tan(degrees / 180 * Math.PI)) * right);
            right = Vector3.Cross(up, d);
            setScreen();
        }

        public void TurnLeft(float degrees)
        {
            up = Vector3.Normalize(up + ((float)Math.Tan(degrees / 180 * Math.PI)) * -1 * right);
            right = Vector3.Cross(up, d);
            setScreen();
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
