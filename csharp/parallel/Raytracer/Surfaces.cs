//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;

namespace Raytracer
{
    static class Surfaces
    {
        // Only works with X-Z plane.
        public static readonly Surface CheckerBoard =
            new Surface(
                (Vector pos) => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
                    ? new Color(1, 1, 1)
                    : new Color(0.02, 0.0, 0.14),
                (Vector pos) => new Color(1, 1, 1),
                (Vector pos) => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0) ? .1 : .5,
                150);

        public static readonly Surface Shiny =
            new Surface(
                (Vector pos) => new Color(1, 1, 1),
                (Vector pos) => new Color(.5, .5, .5),
                (Vector pos) => .7,
                250);

        public static readonly Surface MatteShiny =
            new Surface(
                (Vector pos) => new Color(1, 1, 1),
                (Vector pos) => new Color(.25, .25, .25),
                (Vector pos) => .7,
                250);

    }
}
