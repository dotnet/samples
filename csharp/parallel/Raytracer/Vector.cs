//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;

namespace Raytracer
{
    struct Vector
    {
        public double X;
        public double Y;
        public double Z;

        public Vector(double x, double y, double z) => (X, Y, Z) = (x, y, z);

        public Vector(string str)
        {
            var nums = str.Split(',');
            if (nums.Length != 3) throw new ArgumentException();

            X = double.Parse(nums[0]);
            Y = double.Parse(nums[1]);
            Z = double.Parse(nums[2]);
        }

        public static Vector Times(double n, Vector v) => new Vector(v.X * n, v.Y * n, v.Z * n);
        public static Vector Minus(Vector v1, Vector v2) => new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        public static Vector Plus(Vector v1, Vector v2) => new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        public static double Dot(Vector v1, Vector v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        public static double Mag(Vector v) => Math.Sqrt(Dot(v, v));
        public static Vector Norm(Vector v)
        {
            double mag = Mag(v);
            double div = mag == 0 ? double.PositiveInfinity : 1 / mag;
            return Times(div, v);
        }
        public static Vector Cross(Vector v1, Vector v2) =>
            new Vector(
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X);

        public static bool Equals(Vector v1, Vector v2) =>
            v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;

        public static readonly Vector Null = new Vector(0, 0, 0);
    }
}
