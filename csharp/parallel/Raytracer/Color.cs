//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;

namespace Raytracer
{
    struct Color
    {
        public double R;
        public double G;
        public double B;

        public Color(double r, double g, double b) =>
            (R, G, B) = (r, g, b);

        public Color(string str)
        {
            var nums = str.Split(',');
            if (nums.Length != 3) throw new ArgumentException();

            R = double.Parse(nums[0]);
            G = double.Parse(nums[1]);
            B = double.Parse(nums[2]);
        }


        public static Color Times(double n, Color v) => new Color(n * v.R, n * v.G, n * v.B);
        public static Color Times(Color v1, Color v2) => new Color(v1.R * v2.R, v1.G * v2.G, v1.B * v2.B);

        public static Color Plus(Color v1, Color v2) => new Color(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B);
        public static Color Minus(Color v1, Color v2) => new Color(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B);

        public static readonly Color DefaultColor = new Color(0, 0, 0);
        public static readonly Color Background = DefaultColor;

        public static double Legalize(double d) => d > 1 ? 1 : d;

        public static byte ToByte(double c) => (byte)(255 * Legalize(c));

        public static int ToInt32(double c)
        {
            int r = (int)(255 * c);
            return r > 255 ? 255 : r;
        }

        public int ToInt32() => ToInt32(B) | ToInt32(G) << 8 | ToInt32(R) << 16 | 255 << 24;

        internal System.Drawing.Color ToDrawingColor() => System.Drawing.Color.FromArgb(ToInt32());

        public void ChangeHue(double hue)
        {
            double H, S, L;

            System.Drawing.Color c = System.Drawing.Color.FromArgb(ToInt32());
            S = c.GetSaturation();
            L = c.GetBrightness();
            H = c.GetHue();

            H = hue;
            S = 0.9;
            L = (L - 0.5) * 0.5 + 0.5;

            if (L == 0)
            {
                R = G = B = 0;
            }
            else
            {
                if (S == 0)
                {
                    R = G = B = L;
                }
                else
                {
                    double temp2 = (L <= 0.5) ? L * (1.0 + S) : L + S - L * S;
                    double temp1 = 2.0 * L - temp2;

                    double[] t3 = new double[] { H + 1.0 / 3.0, H, H - 1.0 / 3.0 };
                    double[] clr = new double[] { 0, 0, 0 };

                    for (int i = 0; i < 3; i++)
                    {

                        if (t3[i] < 0) t3[i] += 1.0;
                        if (t3[i] > 1) t3[i] -= 1.0;
                        if (6.0 * t3[i] < 1.0)
                            clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                        else if (2.0 * t3[i] < 1.0)
                            clr[i] = temp2;
                        else if (3.0 * t3[i] < 2.0)
                            clr[i] = temp1 + (temp2 - temp1) * (2.0 / 3.0 - t3[i]) * 6.0;
                        else
                            clr[i] = temp1;
                    }

                    R = clr[0];
                    G = clr[1];
                    B = clr[2];
                }
            }
        }
    }
}
