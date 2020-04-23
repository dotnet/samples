//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Raytracer
{
    class ISect
    {
        public SceneObject Thing;
        public Ray Ray;
        public double Dist;

        public ISect(SceneObject thing, Ray ray, double dist) =>
            (Thing, Ray, Dist) = (thing, ray, dist);

        public static bool IsNull(ISect sect) => sect == null;
        public static readonly ISect Null = null;
    }
}
