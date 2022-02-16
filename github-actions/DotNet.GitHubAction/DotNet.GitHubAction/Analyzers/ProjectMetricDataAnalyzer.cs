using System.Collections.Immutable;

namespace DotNet.GitHubAction.Analyzers;

sealed class ProjectMetricDataAnalyzer
{
    readonly ILogger<ProjectMetricDataAnalyzer> _logger;

    public ProjectMetricDataAnalyzer(ILogger<ProjectMetricDataAnalyzer> logger) => _logger = logger;

    internal async Task<ImmutableArray<(string, CodeAnalysisMetricData)>> AnalyzeAsync(
        ProjectWorkspace workspace, string path, CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();

        if (File.Exists(path))
        {
            _logger.LogInformation($"Computing analytics on {path}.");
        }
        else
        {
            _logger.LogWarning($"{path} doesn't exist.");
            return ImmutableArray<(string, CodeAnalysisMetricData)>.Empty;
        }

        var projects =
            await workspace.LoadProjectAsync(
                path, cancellationToken: cancellation)
                .ConfigureAwait(false);

        var builder = ImmutableArray.CreateBuilder<(string, CodeAnalysisMetricData)>();
        foreach (var project in projects)
        {
            var compilation =
                await project.GetCompilationAsync(cancellation)
                    .ConfigureAwait(false);

            var metricData = await CodeAnalysisMetricData.ComputeAsync(
                compilation!.Assembly,
                new CodeMetricsAnalysisContext(compilation, cancellation))
                    .ConfigureAwait(false);

            builder.Add((project.FilePath!, metricData));
        }

        return builder.ToImmutable();
    }
}
