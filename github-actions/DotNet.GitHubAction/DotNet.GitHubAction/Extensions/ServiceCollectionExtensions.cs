namespace DotNet.GitHubAction.Extensions;

static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddGitHubActionServices(
        this IServiceCollection services) =>
        services.AddSingleton<ProjectMetricDataAnalyzer>()
                .AddDotNetCodeAnalysisServices();
}
