namespace RayTracer
{
    //    Scene, which stores a list of primitives and light sources.It implements a scene-level Intersect
    //method, which loops over the primitives and returns the closest intersection
    class Scene
    {
        private Primitive[] primitives;
        private Light[] lights;

        public Intersection Intersect(Ray ray)
        {
            Intersection nearest = null;
            Intersection next = null;

            for (int i = 0; i < primitives.Length; i++)
            {
                next = primitives[i].Intersect(ray);
                if(next != null && (nearest == null || next.dist < nearest.dist))
                {
                    nearest = next;
                }
            }
            return nearest;
        }
    }
}
