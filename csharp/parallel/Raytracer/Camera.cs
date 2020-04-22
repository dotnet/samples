//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Raytracer
{
    readonly struct Camera
    {
        public readonly Vector Pos;
        public readonly Vector Forward;
        public readonly Vector Up;
        public readonly Vector Right;

        public Camera(Vector pos, Vector forward, Vector up, Vector right) =>
            (Pos, Forward, Up, Right) = (pos, forward, up, right);

        public static Camera Create(Vector pos, Vector lookAt)
        {
            var forward = Vector.Norm(Vector.Minus(lookAt, pos));
            var down = new Vector(0, -1, 0);
            var right = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, down)));
            var up = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, right)));

            return new Camera(pos, forward, up, right);
        }
    }
}
