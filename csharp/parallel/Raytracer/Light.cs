//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Raytracer
{
    readonly struct Light
    {
        public readonly Vector Pos;
        public readonly Color Color;

        public Light(Vector pos, Color color) => (Pos, Color) = (pos, color);
    }
}
