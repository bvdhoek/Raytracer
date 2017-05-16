using System;
using System.Numerics;

namespace RayTracer
{
    internal class Material
    {
        public Vector3 color { get; private set; }
        public bool isMirror { get; private set; } = false;

        internal void SetColor(Vector3 color, bool isMirror)
        {
            this.color = color;
            this.isMirror = isMirror;
        }
    }
}