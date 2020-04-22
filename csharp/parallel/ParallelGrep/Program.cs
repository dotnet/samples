//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

class ParallelGrep
{
    static void Main(string[] args)
    {
        // Parse command-line switches
        bool recursive = args.Contains("/s");
        bool ignoreCase = args.Contains("/i");

        // Get the regex and file wildcards from the command-line
        var nonSwitches = from arg in args where arg.FirstOrDefault() != '/' select arg;
        var regexString = nonSwitches.FirstOrDefault();
        var wildcards = nonSwitches.Skip(1);

        // Create thread-local Regex instances, to prevent contention on Regex's internal lock
        var regex = new ThreadLocal<Regex>(() =>
            new Regex(regexString, RegexOptions.Compiled | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)));

        // Get the list of files to be examined
        var files = from wc in wildcards
                    let dirName = Path.GetDirectoryName(wc)
                    let fileName = Path.GetFileName(wc)
                    from file in Directory.EnumerateFiles(
                        string.IsNullOrWhiteSpace(dirName) ? "." : dirName,
                        string.IsNullOrWhiteSpace(fileName) ? "*.*" : fileName,
                        recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    select file;

        try
        {
            // Traverse the specified files in parallel, and run each line through the Regex, collecting line number info
            // for each match (the Zip call counts the lines in each file)
            var matches = from file in files.AsParallel().AsOrdered().WithMergeOptions(ParallelMergeOptions.NotBuffered)
                          from line in File.ReadLines(file).Zip(Enumerable.Range(1, int.MaxValue), (s, i) => new { Num = i, Text = s, File = file })
                          where regex.Value.IsMatch(line.Text)
                          select line;
            foreach (var line in matches)
            {
                Console.WriteLine($"{line.File}:{line.Num} {line.Text}");
            }
        }
        catch (AggregateException ae)
        {
            ae.Handle(e => { Console.WriteLine(e.Message); return true; });
        }
    }
}
