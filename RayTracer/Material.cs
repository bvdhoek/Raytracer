using System;
using System.Numerics;

namespace RayTracer
{
    public class Material
    {
        public Vector3 color { get; private set; }
        public bool isMirror { get; private set; }
        public bool isDielectic { get; private set; }
        public bool isShiny { get; private set; }

        // the extend to which this material reflects light
        public float reflectiveness { get; private set; }
        // the extend to which this material lets light through
        public float transparency { get; private set; }
        // the extend to which this material is shiny
        public float shine { get; private set; }
        // diffuseness. equals 1 - (all other components)
        public float diffuseness { get; private set; }

        public float refractionIndex = 1.5f; // glass

        private bool isCheckered;
        internal Vector3 absorbtion;

        public Material()
        {
            this.diffuseness = 1;
        }

        internal void SetColor(Vector3 color, float reflectiveness = 0, float transparency = 0, float shine = 0)
        {
            if((reflectiveness + transparency + shine) >= 1.01)
            {
                throw new Exception("Material components total cannot exceed 1!");
            }

            this.color = color;
            this.reflectiveness = reflectiveness;
            this.transparency = transparency;
            this.shine = shine;

            if (reflectiveness > 0) this.isMirror = true;
            if (transparency > 0) this.isDielectic = true;
            if (shine > 0) this.isShiny = true;

            this.diffuseness = 1 - (reflectiveness + transparency + shine);
        }
    }
}