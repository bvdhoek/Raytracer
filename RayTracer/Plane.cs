﻿using System;
using System.Numerics;

namespace RayTracer
{
    class Plane : Primitive
    {
        public bool isCheckered;

        // normal of the plane
        private Vector3 normal;

        public Plane(Vector3 normal, Vector3 origin) : base(origin, new Vector3(1, 1, 0), 0.5f)
        {
            this.normal = normal;
        }

        public Plane(Vector3 normal, Vector3 origin, Vector3 color) : base(origin, color)
        {
            this.normal = normal;
        }

        public Plane(Vector3 normal,
            Vector3 origin,
            Vector3 color,
            float reflectiveness = 0,
            float transparency = 0,
            float shine = 0
            ) : base(origin, color, reflectiveness, transparency, shine)
        {
            this.normal = normal;
        }

        public override Intersection Intersect(Ray ray)
        {
            float denom = Vector3.Dot(normal, ray.direction);
            Vector3 rayToPlane = origin - ray.origin;
            float t = Vector3.Dot(rayToPlane, normal) / denom;
            if (t >= 0)
            {
                if (t < ray.t)
                {
                    ray.t = t;
                    return new Intersection(this, normal, ray.t * ray.direction, ray.t);
                }
            }
            return null;
        }

        internal override Vector3 GetColor(Vector3 location)
        {
            if (isCheckered)
            {
                if (location.X % 1 < 0.5 && location.X >= 0 && Math.Abs(location.Z) % 1 < 0.5
                    || location.X % 1 > 0.5 && location.X >= 0 && Math.Abs(location.Z) % 1 > 0.5
                    || location.X % 1 < -0.5 && Math.Abs(location.Z) % 1 < 0.5
                    || location.X % 1 > -0.5 && location.X < 0 && Math.Abs(location.Z) % 1 > 0.5)
                    return new Vector3(0, 0, 0);
                else return new Vector3(1, 1, 1);
            }
            else return this.material.color;
        }
    }
}
