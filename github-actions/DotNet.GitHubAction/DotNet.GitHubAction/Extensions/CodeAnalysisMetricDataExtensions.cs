using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeMetrics;

namespace DotNet.GitHubAction.Extensions
{
    static class CodeAnalysisMetricDataExtensions
    {
        internal static string ToCyclomaticComplexityEmoji(this CodeAnalysisMetricData metric) =>
            metric.CyclomaticComplexity switch
            {
                >= 0 and <= 7 => ":heavy_check_mark:",
                8 or 9 => ":warning:",
                10 or 11 => ":radioactive:",
                12 or 14 => ":x:",
                _ => ":feelsgood:"
            };

        internal static int CountNamespaces(this CodeAnalysisMetricData metric) =>
            metric.CountKind(SymbolKind.Namespace);

        internal static int CountNamedTypes(this CodeAnalysisMetricData metric) =>
            metric.CountKind(SymbolKind.NamedType);

        static int CountKind(this CodeAnalysisMetricData metric, SymbolKind kind) =>
            metric.Children
                .Flatten(child => child.Children)
                .Count(child => child.Symbol.Kind == kind);

        internal static string FindHighestCyclomaticComplexity(this CodeAnalysisMetricData metric) =>
            metric.Children
                .Flatten(child => child.Children)
                .Where(child =>
                    child.Symbol.Kind is not SymbolKind.Assembly
                    and not SymbolKind.Namespace
                    and not SymbolKind.NamedType)
                .Select(m => (Metric: m, m.CyclomaticComplexity))
                .OrderByDescending(_ => _.CyclomaticComplexity)
                .Select(_ => $"{_.CyclomaticComplexity} {_.Metric.ToCyclomaticComplexityEmoji()}")
                .First();

        static IEnumerable<TSource> Flatten<TSource>(
            this IEnumerable<TSource> parent, Func<TSource, IEnumerable<TSource>> childSelector) =>
            parent.SelectMany(
                source => childSelector(source).Flatten(childSelector))
                .Concat(parent);
    }
}
