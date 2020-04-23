//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Raytracer
{
    abstract class SceneObject
    {
        public Surface Surface;

        public abstract ISect Intersect(Ray ray);

        public abstract Vector Normal(Vector pos);

        public SceneObject(Surface surface) => Surface = surface;
    }
}
