//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;

namespace Raytracer
{
    readonly struct Surface
    {
        public readonly Func<Vector, Color> Diffuse;
        public readonly Func<Vector, Color> Specular;
        public readonly Func<Vector, double> Reflect;
        public readonly double Roughness;

        public Surface(
            Func<Vector, Color> diffuse,
            Func<Vector, Color> specular,
            Func<Vector, double> reflect,
            double roughness)
        {
            Diffuse = diffuse;
            Specular = specular;
            Reflect = reflect;
            Roughness = roughness;
        }

    }
}
