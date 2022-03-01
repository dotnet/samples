using Markdown;

namespace DotNet.GitHubAction.Extensions;

static class CodeMetricsReportExtensions
{
    internal static string ToMarkDownBody(
        this Dictionary<string, CodeAnalysisMetricData> metricData,
        ActionInputs actionInputs)
    {
        MarkdownDocument document = new();

        DisableMarkdownLinterAndCaptureConfig(document);

        document.AppendHeader("Code Metrics", 1);

        document.AppendParagraph(
            $"This file is dynamically maintained by a bot, *please do not* edit this by hand. It represents various [code metrics](https://aka.ms/dotnet/code-metrics), such as cyclomatic complexity, maintainability index, and so on.");

        List<(string Id, string ClassName, string MermaidCode)> classDiagrams = new();
        foreach ((string filePath, CodeAnalysisMetricData assemblyMetric)
            in metricData.OrderBy(md => md.Key))
        {
            var (assemblyId, assemblyDisplayName, assemblyLink, assemblyHighestComplexity) =
                ToIdAndAnchorPair(assemblyMetric);

            document.AppendParagraph($"<div id='{assemblyId}'></div>");
            document.AppendHeader($"{assemblyDisplayName} {assemblyHighestComplexity.emoji}", 2);

            document.AppendParagraph(
                $"The *{Path.GetFileName(filePath)}* project file contains:");

            static string FormatComplexity((int complexity, string emoji) highestComplexity) =>
                $"{highestComplexity.complexity} {highestComplexity.emoji}";

            document.AppendList(
                new MarkdownTextListItem($"{assemblyMetric.CountNamespaces():#,0} namespaces."),
                new MarkdownTextListItem($"{assemblyMetric.CountNamedTypes():#,0} named types."),
                new MarkdownTextListItem($"{assemblyMetric.SourceLines:#,0} total lines of source code."),
                new MarkdownTextListItem($"Approximately {assemblyMetric.ExecutableLines:#,0} lines of executable code."),
                new MarkdownTextListItem($"The highest cyclomatic complexity is {FormatComplexity(assemblyHighestComplexity)}."));

            foreach (var namespaceMetric
                in assemblyMetric.Children.Where(child => child.Symbol.Kind == SymbolKind.Namespace)
                .OrderBy(md => md.Symbol.Name))
            {
                var (namespaceId, namespaceSymbolName, namespaceLink, namespaceHighestComplexity) =
                    ToIdAndAnchorPair(namespaceMetric);
                OpenCollapsibleSection(
                    document, namespaceId, namespaceSymbolName, namespaceHighestComplexity.emoji);

                document.AppendParagraph(
                    $"The `{namespaceSymbolName}` namespace contains {namespaceMetric.Children.Length} named types.");

                document.AppendList(
                    new MarkdownTextListItem($"{namespaceMetric.CountNamedTypes():#,0} named types."),
                    new MarkdownTextListItem($"{namespaceMetric.SourceLines:#,0} total lines of source code."),
                    new MarkdownTextListItem($"Approximately {namespaceMetric.ExecutableLines:#,0} lines of executable code."),
                    new MarkdownTextListItem($"The highest cyclomatic complexity is {FormatComplexity(namespaceHighestComplexity)}."));

                foreach (var classMetric in namespaceMetric.Children
                    .OrderBy(md => md.Symbol.Name))
                {
                    var (classId, classSymbolName, classLink, namedTypeHighestComplexity) = ToIdAndAnchorPair(classMetric);
                    OpenCollapsibleSection(
                        document, classId, classSymbolName, namedTypeHighestComplexity.emoji);

                    document.AppendList(
                        new MarkdownTextListItem($"The `{classSymbolName}` contains {classMetric.Children.Length} members."),
                        new MarkdownTextListItem($"{classMetric.SourceLines:#,0} total lines of source code."),
                        new MarkdownTextListItem($"Approximately {classMetric.ExecutableLines:#,0} lines of executable code."),
                        new MarkdownTextListItem($"The highest cyclomatic complexity is {FormatComplexity(namedTypeHighestComplexity)}."));

                    MarkdownTableHeader tableHeader = new(
                        new("Member kind", MarkdownTableTextAlignment.Center),
                        new("Line number", MarkdownTableTextAlignment.Center),
                        new("Maintainability index", MarkdownTableTextAlignment.Center),
                        new("Cyclomatic complexity", MarkdownTableTextAlignment.Center),
                        new("Depth of inheritance", MarkdownTableTextAlignment.Center),
                        new("Class coupling", MarkdownTableTextAlignment.Center),
                        new("Lines of source / executable code", MarkdownTableTextAlignment.Center));

                    List<MarkdownTableRow> rows = new();
                    foreach (var memberMetric in classMetric.Children.OrderBy(md => md.Symbol.Name))
                    {
                        rows.Add(ToTableRowFrom(memberMetric, actionInputs));
                    }

                    document.AppendTable(tableHeader, rows);

                    if (classSymbolName is not "<Program>$")
                    {
                        var encodedName = HttpUtility.HtmlEncode(classSymbolName);
                        var id = $"{encodedName}-class-diagram";
                        var linkToClassDiagram = $"<a href=\"#{id}\">:link: to `{encodedName}` class diagram</a>";
                        document.AppendParagraph(linkToClassDiagram);
                        classDiagrams.Add((id, classSymbolName, classMetric.ToMermaidClassDiagram(classSymbolName)));
                    }

                    document.AppendParagraph(namespaceLink); // Links back to the parent namespace in the MD doc

                    CloseCollapsibleSection(document);
                }

                CloseCollapsibleSection(document);
            }

            document.AppendParagraph(assemblyLink); // Links back to the parent assembly in the MD doc
        }

        AppendMetricDefinitions(document);
        AppendMermaidClassDiagrams(document, classDiagrams);
        AppendMaintainedByBotMessage(document);
        RestoreMarkdownLinter(document);

        return document.ToString();
    }

    static void AppendMetricDefinitions(MarkdownDocument document)
    {
        document.AppendHeader("Metric definitions", 2);

        MarkdownList markdownList = new();
        foreach ((string columnHeader, string defintion)
            in new (string, string)[]
            {
                ("Maintainability index", "Measures ease of code maintenance. Higher values are better."),
                ("Cyclomatic complexity", "Measures the number of branches. Lower values are better."),
                ("Depth of inheritance", "Measures length of object inheritance hierarchy. Lower values are better."),
                ("Class coupling", "Measures the number of classes that are referenced. Lower values are better."),
                ("Lines of source code", "Exact number of lines of source code. Lower values are better."),
                ("Lines of executable code", "Approximates the lines of executable code. Lower values are better.")
            })
        {
            MarkdownText header = new($"**{columnHeader}**");
            MarkdownText text = new(defintion);

            markdownList.AddItem($"{header}: {text}");
        }

        document.AppendList(markdownList);
    }

    static void AppendMermaidClassDiagrams(
        MarkdownDocument document, List<(string Id, string Class, string MermaidCode)> diagrams)
    {
        document.AppendHeader("Mermaid class diagrams", 2);

        foreach (var (id, className, code) in diagrams)
        {
            document.AppendParagraph($"<div id=\"{id}\"></div>");
            document.AppendHeader($"`{className}` class diagram", 5);
            document.AppendCode("mermaid", code);
        }
    }

    static void AppendMaintainedByBotMessage(MarkdownDocument document) =>
        document.AppendParagraph(
            new MarkdownEmphasis("This file is maintained by a bot."));

    static MarkdownTableRow ToTableRowFrom(
        CodeAnalysisMetricData metric,
        ActionInputs actionInputs)
    {
        var lineNumberUrl = ToLineNumberUrl(metric.Symbol, metric.ToDisplayName(), actionInputs);
        var maintainability = metric.MaintainabilityIndex.ToString(CultureInfo.InvariantCulture);
        var cyclomaticComplexity = metric.CyclomaticComplexity.ToString(CultureInfo.InvariantCulture);
        var complexityCell = $"{cyclomaticComplexity} {metric.ToCyclomaticComplexityEmoji()}";
        var depthOfInheritance = metric.DepthOfInheritance.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
        var classCoupling = metric.CoupledNamedTypes.Count.ToString(CultureInfo.InvariantCulture);
        var linesOfCode =
            $"{metric.SourceLines:#,0} / {metric.ExecutableLines:#,0}";

        return new(
            metric.Symbol.Kind.ToString(),
            lineNumberUrl,
            maintainability,
            complexityCell,
            depthOfInheritance,
            classCoupling,
            linesOfCode);
    }

    static (string elementId, string displayName, string anchorLink, (int highestComplexity, string emoji)) ToIdAndAnchorPair(
        CodeAnalysisMetricData metric)
    {
        var displayName = metric.ToDisplayName();
        var id = PrepareElementId(displayName);
        var highestComplexity = metric.FindHighestCyclomaticComplexity();
        var anchorLink = $"<a href=\"#{id}\">:top: back to {HttpUtility.HtmlEncode(displayName)}</a>";

        return (id, displayName, anchorLink, (highestComplexity.Complexity, highestComplexity.Emoji ?? ":question:"));
    }

    static IMarkdownDocument OpenCollapsibleSection(
        IMarkdownDocument document, string elementId, string symbolName, string highestComplexity) =>
        document.AppendParagraph($@"<details>
<summary>
  <strong id=""{PrepareElementId(elementId)}"">
    {HttpUtility.HtmlEncode(symbolName)} {highestComplexity}
  </strong>
</summary>
<br>");

    static string PrepareElementId(string value) =>
        value.ToLower()
            .Replace('.', '-')
            .Replace("<", "")
            .Replace(">", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(' ', '+');

    static IMarkdownDocument CloseCollapsibleSection(IMarkdownDocument document) =>
        document.AppendParagraph("</details>");

    static IMarkdownDocument DisableMarkdownLinterAndCaptureConfig(
        IMarkdownDocument document) =>
        document.AppendParagraph(@"<!-- markdownlint-capture -->
<!-- markdownlint-disable -->");

    static IMarkdownDocument RestoreMarkdownLinter(
        IMarkdownDocument document) =>
        document.AppendParagraph(@"<!-- markdownlint-restore -->");

    static string ToLineNumberUrl(
        ISymbol symbol, string symbolDisplayName, ActionInputs actionInputs)
    {
        var location = symbol.Locations.FirstOrDefault();
        if (location is null)
        {
            return "N/A";
        }

        var lineNumber = location.GetLineSpan().StartLinePosition.Line + 1;

        if (location.SourceTree is { FilePath.Length: > 0 })
        {
            var fullPath = location.SourceTree?.FilePath;
            var relativePath =
                Path.GetRelativePath(actionInputs.WorkspaceDirectory, fullPath!)
                    .Replace("\\", "/");
            var lineNumberFileReference =
                $"https://github.com/{actionInputs.Owner}/{actionInputs.Name}/blob/{actionInputs.Branch}/{relativePath}#L{lineNumber}";
            
            // Must force anchor link, as GitHub assumes site-relative links.
            return $"<a href='{lineNumberFileReference}' title='{symbolDisplayName}'>{lineNumber:#,0}</a>";
        }

        return $"[{lineNumber:#,0}](# \"{symbolDisplayName}\")";
    }
}
