//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;

namespace Raytracer
{
    class Sphere : SceneObject
    {
        public Vector Center;
        public double Radius;

        public Sphere(Vector center, double radius, Surface surface) : base(surface) =>
            (Center, Radius) = (center, radius);

        public override ISect Intersect(Ray ray)
        {
            var eo = Vector.Minus(Center, ray.Start);
            var v = Vector.Dot(eo, ray.Dir);
            double dist;
            if (v < 0)
            {
                dist = 0;
            }
            else
            {
                double disc = Math.Pow(Radius, 2) - (Vector.Dot(eo, eo) - Math.Pow(v, 2));
                dist = disc < 0 ? 0 : v - Math.Sqrt(disc);
            }

            return dist == 0 ? ISect.Null : new ISect(this, ray, dist);
        }

        public override Vector Normal(Vector pos) => Vector.Norm(Vector.Minus(pos, Center));
    }
}
