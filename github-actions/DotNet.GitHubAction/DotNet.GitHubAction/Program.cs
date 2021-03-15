using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DotNet.CodeAnalysis;
using DotNet.GitHubAction;
using DotNet.GitHubAction.Analyzers;
using DotNet.GitHubAction.Extensions;
using Microsoft.CodeAnalysis.CodeMetrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static CommandLine.Parser;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => services.AddGitHubActionServices())
    .Build();

static TService Get<TService>(IHost host)
    where TService : notnull =>
    host.Services.GetRequiredService<TService>();

static async Task StartAnalysisAsync(ActionInputs inputs, IHost host)
{
    using ProjectWorkspace workspace = Get<ProjectWorkspace>(host);
    using CancellationTokenSource tokenSource = new();

    Console.CancelKeyPress += delegate
    {
        tokenSource.Cancel();
    };

    var projectAnalyzer = Get<ProjectMetricDataAnalyzer>(host);

    Matcher matcher = new();
    matcher.AddIncludePatterns(new[] { "**/*.csproj", "**/*.vbproj" });

    Dictionary<string, CodeAnalysisMetricData> metricData = new(StringComparer.OrdinalIgnoreCase);
    var projects = matcher.GetResultsInFullPath(inputs.Directory);

    foreach (var project in projects)
    {
        var metrics =
            await projectAnalyzer.AnalyzeAsync(
                workspace, project, tokenSource.Token);

        foreach (var (path, metric) in metrics)
        {
            metricData[path] = metric;
        }
    }

    StringBuilder summary = new();
    if (metricData is { Count: > 0 })
    {
        var fileName = "CODE_METRICS.md";
        var fullPath = Path.Combine(inputs.Directory, fileName);
        var logger = Get<ILoggerFactory>(host).CreateLogger(nameof(StartAnalysisAsync));
        var fileExists = File.Exists(fullPath);

        logger.LogInformation(
            $"{(fileExists ? "Updating" : "Creating")} {fileName} markdown file with latest code metric data.");

        summary.Append(
            $"{(fileExists ? "Updated" : "Created")} {fileName} file, analyzed metrics for {metricData.Count} projects.");

        await File.WriteAllTextAsync(
            fullPath,
            metricData.ToMarkDownBody(inputs.Directory, inputs.Branch),
            tokenSource.Token);
    }
    else
    {
        summary.Append("No metrics were determined.");
    }

    // https://docs.github.com/actions/reference/workflow-commands-for-github-actions#setting-an-output-parameter
    Console.WriteLine($"::set-output name=summary-message::{summary}");

    Environment.Exit(0);
}

var parser = Default.ParseArguments<ActionInputs>(() => new(), args);
parser.WithNotParsed(
    errors =>
    {
        Get<ILoggerFactory>(host)
            .CreateLogger("DotNet.GitHubAction.Program")
            .LogError(
                string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        
        Environment.Exit(2);
    });

await parser.WithParsedAsync(options => StartAnalysisAsync(options, host));
await host.RunAsync();
