//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Raytracer
{
    class Plane : SceneObject
    {
        public Vector Norm;
        public double Offset;

        public Plane(Vector norm, double offset, Surface surface) : base(surface) =>
            (Norm, Offset) = (norm, offset);

        public override ISect Intersect(Ray ray)
        {
            var denom = Vector.Dot(Norm, ray.Dir);
            return denom > 0 ? ISect.Null : new ISect(this, ray, (Vector.Dot(Norm, ray.Start) + Offset) / -denom);
        }

        public override Vector Normal(Vector pos) => Norm;
    }
}
