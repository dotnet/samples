//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Raytracer
{
    internal sealed class RayTracer
    {
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private readonly Dictionary<int, double> _numToHueShiftLookup = new Dictionary<int, double>();
        private readonly Random _rand = new Random();

        private const int MaxDepth = 5;

        internal readonly Scene _defaultScene = CreateDefaultScene();

        public RayTracer(int screenWidth, int screenHeight) =>
            (_screenWidth, _screenHeight) = (screenWidth, screenHeight);

        internal void RenderSequential(Scene scene, int[] rgb)
        {
            for (int y = 0; y < _screenHeight; y++)
            {
                int stride = y * _screenWidth;
                Camera camera = scene.Camera;
                for (int x = 0; x < _screenWidth; x++)
                {
                    Color color = TraceRay(new Ray(camera.Pos, GetPoint(x, y, camera)), scene, 0);
                    rgb[x + stride] = color.ToInt32();
                }
            }
        }

        internal void RenderParallel(Scene scene, int[] rgb, ParallelOptions options)
        {
            try
            {
                Parallel.For(0, _screenHeight, options, y =>
                {
                    int stride = y * _screenWidth;
                    Camera camera = scene.Camera;
                    for (int x = 0; x < _screenWidth; x++)
                    {
                        Color color = TraceRay(new Ray(camera.Pos, GetPoint(x, y, camera)), scene, 0);
                        rgb[x + stride] = color.ToInt32();
                    }
                });
            }
            catch (OperationCanceledException)
            {
                // Catch this to prevent the UI from crashing, we know a cancellation will occur.
            }
        }

        internal void RenderParallelShowingThreads(Scene scene, int[] rgb, ParallelOptions options)
        {
            try
            {
                int id = 0;
                Parallel.For<double>(0, _screenHeight, options, () => GetHueShift(Interlocked.Increment(ref id)), (y, state, hue) =>
                {
                    int stride = y * _screenWidth;
                    Camera camera = scene.Camera;
                    for (int x = 0; x < _screenWidth; x++)
                    {
                        Color color = TraceRay(new Ray(camera.Pos, GetPoint(x, y, camera)), scene, 0);
                        color.ChangeHue(hue);
                        rgb[x + stride] = color.ToInt32();
                    }
                    return hue;
                },
                hue => Interlocked.Decrement(ref id));
            }
            catch (OperationCanceledException)
            {
                // Catch this to prevent the UI from crashing, we know a cancellation will occur.
            }
        }

        private double GetHueShift(int id)
        {
            double shift;
            lock (_numToHueShiftLookup)
            {
                if (!_numToHueShiftLookup.TryGetValue(id, out shift))
                {
                    shift = _rand.NextDouble();
                    _numToHueShiftLookup.Add(id, shift);
                }
            }
            return shift;
        }

        static Scene CreateDefaultScene()
        {
            SceneObject[] things =
            {
                new Sphere( new Vector(-0.5,1,1.5), 0.5, Surfaces.MatteShiny),
                new Sphere( new Vector(0,1,-0.25), 1, Surfaces.Shiny),
                new Plane( new Vector(0,1,0), 0, Surfaces.CheckerBoard)
            };
            Light[] lights =
            {
                new Light(new Vector(-2,2.5,0),new Color(.5,.45,.41)),
                new Light(new Vector(2,4.5,2), new Color(.99,.95,.8))
            };

            return new Scene(things, lights, Camera.Create(new Vector(2.75, 2, 3.75), new Vector(-0.6, .5, 0)));
        }


        private ISect MinIntersection(Ray ray, Scene scene)
        {
            var min = ISect.Null;
            foreach (var obj in scene.Things)
            {
                var isect = obj.Intersect(ray);
                if (!ISect.IsNull(isect))
                {
                    if (ISect.IsNull(min) || min.Dist > isect.Dist)
                    {
                        min = isect;
                    }
                }
            }
            return min;
        }

        private double TestRay(Ray ray, Scene scene)
        {
            var isect = MinIntersection(ray, scene);
            return ISect.IsNull(isect) ? 0 : isect.Dist;
        }

        private Color TraceRay(Ray ray, Scene scene, int depth)
        {
            var isect = MinIntersection(ray, scene);
            return ISect.IsNull(isect) ? Color.Background : Shade(isect, scene, depth);
        }

        private Color GetNaturalColor(SceneObject thing, Vector pos, Vector norm, Vector rd, Scene scene)
        {
            var ret = new Color(0, 0, 0);
            foreach (Light light in scene.Lights)
            {
                Vector ldis = Vector.Minus(light.Pos, pos);
                Vector livec = Vector.Norm(ldis);
                double neatIsect = TestRay(new Ray(pos, livec), scene);
                bool isInShadow = !(neatIsect > Vector.Mag(ldis) || neatIsect == 0);
                if (!isInShadow)
                {
                    double illum = Vector.Dot(livec, norm);
                    Color lcolor = illum > 0 ? Color.Times(illum, light.Color) : new Color(0, 0, 0);
                    double specular = Vector.Dot(livec, Vector.Norm(rd));
                    Color scolor = specular > 0 ? Color.Times(Math.Pow(specular, thing.Surface.Roughness), light.Color) : new Color(0, 0, 0);
                    ret = Color.Plus(ret, Color.Plus(Color.Times(thing.Surface.Diffuse(pos), lcolor),
                                                     Color.Times(thing.Surface.Specular(pos), scolor)));
                }
            }
            return ret;
        }

        private Color GetReflectionColor(
            SceneObject thing, Vector pos, Vector norm, Vector rd, Scene scene, int depth) =>
            Color.Times(thing.Surface.Reflect(pos), TraceRay(new Ray(pos, rd), scene, depth + 1));

        private Color Shade(ISect isect, Scene scene, int depth)
        {
            var d = isect.Ray.Dir;
            var pos = Vector.Plus(Vector.Times(isect.Dist, isect.Ray.Dir), isect.Ray.Start);
            var normal = isect.Thing.Normal(pos);
            var reflectDir = Vector.Minus(d, Vector.Times(2 * Vector.Dot(normal, d), normal));
            var ret = Color.Plus(Color.DefaultColor, GetNaturalColor(isect.Thing, pos, normal, reflectDir, scene));
            if (depth >= MaxDepth)
            {
                return Color.Plus(ret, new Color(.5, .5, .5));
            }

            return Color.Plus(ret, GetReflectionColor(isect.Thing, Vector.Plus(pos, Vector.Times(.001, reflectDir)), normal, reflectDir, scene, depth));
        }

        private double RecenterX(double x) => (x - _screenWidth / 2.0) / (2.0 * _screenWidth);
        private double RecenterY(double y) => -(y - _screenHeight / 2.0) / (2.0 * _screenHeight);

        private Vector GetPoint(double x, double y, Camera camera) =>
            Vector.Norm(
                Vector.Plus(
                    camera.Forward,
                    Vector.Plus(
                        Vector.Times(RecenterX(x), camera.Right),
                        Vector.Times(RecenterY(y), camera.Up))));
    }
}
