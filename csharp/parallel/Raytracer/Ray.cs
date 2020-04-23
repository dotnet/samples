//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Raytracer
{
    readonly struct Ray
    {
        public readonly Vector Start;
        public readonly Vector Dir;

        public Ray(Vector start, Vector dir) => (Start, Dir) = (start, dir);
    }
}
