﻿using System.Numerics;

namespace RayTracer
{
    class Intersection
    {
        // Primitive we intersected
        public Primitive primitive { get; private set; }
        
        // normal vector at intersection point
        public Vector3 normal { get; private set; }

        // Location of intersection with the primitive
        public Vector3 intersectionPoint { get; private set; }
        
        // distance from origin of the ray to intersection
        public float dist;

        public Intersection(Primitive p, Vector3 normal, Vector3 intersectionPoint, float dist)
        {
            primitive = p;
            this.normal = normal;
            this.intersectionPoint = intersectionPoint;
            this.dist = dist;
        }
    }
}