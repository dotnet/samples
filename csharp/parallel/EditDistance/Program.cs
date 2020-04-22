//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Algorithms;

class Program
{
    static void Main()
    {
        var rand = new Random();
        var sw = new Stopwatch();

        int result;
        while (true)
        {
            string s1 = GenerateRandomString(rand);
            string s2 = GenerateRandomString(rand);

            sw.Restart();
            result = SerialEditDistance(s1, s2);
            sw.Stop();
            Console.WriteLine($"Serial  :\t{result}\t{sw.Elapsed}");

            sw.Restart();
            result = ParallelEditDistance(s1, s2);
            sw.Stop();
            Console.WriteLine($"Parallel:\t{result}\t{sw.Elapsed}");

            Console.WriteLine("-------------------------------------------------------");
            GC.Collect();
        }
    }

    private static string GenerateRandomString(Random rand)
    {
        const int LEN = 10_000;
        var sb = new StringBuilder(LEN);
        for (int i = 0; i < LEN; i++) sb.Append((char)('a' + rand.Next(0, 26)));
        return sb.ToString();
    }

    private static int SerialEditDistance(string s1, string s2)
    {
        int[,] dist = new int[s1.Length + 1, s2.Length + 1];
        for (int i = 0; i <= s1.Length; i++) dist[i, 0] = i;
        for (int j = 0; j <= s2.Length; j++) dist[0, j] = j;

        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                dist[i, j] = (s1[i - 1] == s2[j - 1]) ?
                    dist[i - 1, j - 1] :
                    1 + Math.Min(dist[i - 1, j],
                        Math.Min(dist[i, j - 1],
                                 dist[i - 1, j - 1]));
            }
        }

        return dist[s1.Length, s2.Length];
    }

    private static int ParallelEditDistance(string s1, string s2)
    {
        int[,] dist = new int[s1.Length + 1, s2.Length + 1];
        for (int i = 0; i <= s1.Length; i++) dist[i, 0] = i;
        for (int j = 0; j <= s2.Length; j++) dist[0, j] = j;
        int numBlocks = Environment.ProcessorCount * 4;

        ParallelAlgorithms.Wavefront(
            s1.Length, s2.Length,
            numBlocks, numBlocks,
            (start_i, end_i, start_j, end_j) =>
        {
            for (int i = start_i + 1; i <= end_i; i++)
            {
                for (int j = start_j + 1; j <= end_j; j++)
                {
                    dist[i, j] = (s1[i - 1] == s2[j - 1]) ?
                        dist[i - 1, j - 1] :
                        1 + Math.Min(dist[i - 1, j],
                            Math.Min(dist[i, j - 1],
                                     dist[i - 1, j - 1]));
                }
            }
        });

        return dist[s1.Length, s2.Length];
    }
}
