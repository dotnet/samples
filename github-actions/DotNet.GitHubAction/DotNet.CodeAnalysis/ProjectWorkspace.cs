using AnalysisProject = Microsoft.CodeAnalysis.Project;
using MSBProject = Microsoft.Build.Evaluation.Project;
using MSBProjectIntance = Microsoft.Build.Execution.ProjectInstance;

namespace DotNet.CodeAnalysis;

/// Inspired by:
/// https://github.com/dotnet/roslyn/blob/main/src/Workspaces/Core/MSBuild/MSBuild/MSBuildWorkspace.cs
public class ProjectWorkspace : IDisposable
{
    BuildManager _buildManager = BuildManager.DefaultBuildManager;

    readonly AdhocWorkspace _workspace = new();
    readonly HostWorkspaceServices _workspaceServices = null!;
    readonly ProjectLoader _projectLoader;
    readonly ILogger<ProjectWorkspace> _logger;
    readonly Dictionary<string, ProjectItem> _documents = new(StringComparer.OrdinalIgnoreCase);

    static readonly char[] s_directorySplitChars =
        new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

    static readonly ImmutableDictionary<string, string> s_defaultGlobalProperties =
        new Dictionary<string, string>()
        {
            ["DesignTimeBuild"] = bool.TrueString,
            ["NonExistentFile"] = "__NonExistentSubDir__\\__NonExistentFile__",
            ["BuildProjectReferences"] = bool.FalseString,
            ["BuildingProject"] = bool.FalseString,
            ["ProvideCommandLineArgs"] = bool.TrueString,
            ["SkipCompilerExecution"] = bool.TrueString,
            ["ContinueOnError"] = "ErrorAndContinue",
            ["ShouldUnsetParentConfigurationAndPlatform"] = bool.FalseString
        }.ToImmutableDictionary();

    public ProjectWorkspace(
        ProjectLoader projectLoader, ILogger<ProjectWorkspace> logger) =>
        (_projectLoader, _logger, _workspaceServices) = (projectLoader, logger, _workspace.Services);

    public async Task<ImmutableArray<AnalysisProject>> LoadProjectAsync(
        string path, CancellationToken cancellationToken)
    {
        if (File.Exists(path))
        {
            var projectExtension = Path.GetExtension(path);
            var projectDirectory = Path.GetDirectoryName(path)!;
            var language = projectExtension switch
            {
                ".csproj" => LanguageNames.CSharp,
                ".vbproj" => LanguageNames.VisualBasic,

                _ => throw new ArgumentException("Unknown project file, .")
            };

            var project = _projectLoader.LoadProject(path);
            var builder = ImmutableArray.CreateBuilder<AnalysisProject>();

            var buildProjectCollection = new ProjectCollection(s_defaultGlobalProperties);
            var buildParameters = new BuildParameters(buildProjectCollection);

            _buildManager.BeginBuild(buildParameters);

            try
            {
                var projectInfos = await LoadProjectInfosAsync(project, language, projectDirectory, cancellationToken);
                foreach (var projectInfo in projectInfos)
                {
                    builder.Add(_workspace.AddProject(projectInfo));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                _buildManager.EndBuild();
            }

            return builder.ToImmutable();
        }

        return ImmutableArray<AnalysisProject>.Empty;
    }

    async Task<ImmutableArray<ProjectInfo>> LoadProjectInfosAsync(
        MSBProject project, string language, string projectDirectory, CancellationToken cancellationToken)
    {
        var targetFrameworkValue = project.GetPropertyValue("TargetFramework");
        var targetFrameworksValue = project.GetPropertyValue("TargetFrameworks");

        if (string.IsNullOrEmpty(targetFrameworkValue) && !string.IsNullOrEmpty(targetFrameworksValue))
        {
            var targetFrameworks = targetFrameworksValue.Split(';');
            var results = ImmutableArray.CreateBuilder<ProjectInfo>(targetFrameworks.Length);

            if (!project.GlobalProperties.TryGetValue(
                "TargetFramework", out var initialGlobalTargetFrameworkValue))
            {
                initialGlobalTargetFrameworkValue = null;
            }

            foreach (var targetFramework in targetFrameworks)
            {
                project.SetGlobalProperty("TargetFramework", targetFramework);
                project.ReevaluateIfNecessary();

                var projectFileInfo =
                    await BuildProjectFileInfoAsync(project, language, projectDirectory, cancellationToken).ConfigureAwait(false);

                var projectInfo = await CreateProjectInfoAsync(projectFileInfo, projectDirectory);

                results.Add(projectInfo);
            }

            if (initialGlobalTargetFrameworkValue is null)
            {
                project.RemoveGlobalProperty("TargetFramework");
            }
            else
            {
                project.SetGlobalProperty("TargetFramework", initialGlobalTargetFrameworkValue);
            }

            project.ReevaluateIfNecessary();

            return results.ToImmutable();
        }
        else
        {
            var projectFileInfo =
                await BuildProjectFileInfoAsync(project, language, projectDirectory, cancellationToken).ConfigureAwait(false);

            var projectInfo = await CreateProjectInfoAsync(projectFileInfo, projectDirectory);

            return ImmutableArray.Create(projectInfo);
        }
    }

    async Task<ProjectInfo> CreateProjectInfoAsync(ProjectFileInfo projectFileInfo, string projectDirectory)
    {
        var projectId = ProjectId.CreateNewId(debugName: projectFileInfo.FilePath);

        var language = projectFileInfo.Language;
        var projectPath = projectFileInfo.FilePath;

        var projectName = Path.GetFileNameWithoutExtension(projectPath);
        if (!string.IsNullOrWhiteSpace(projectFileInfo.TargetFramework))
        {
            projectName += "(" + projectFileInfo.TargetFramework + ")";
        }

        var version = VersionStamp.Create(
            FileUtilities.GetFileTimeStamp(projectPath));

        if (projectFileInfo.IsEmpty)
        {
            var assembly = GetAssemblyNameFromProjectPath(projectPath);

            return ProjectInfo.Create(
                projectId,
                version,
                projectName,
                assemblyName: assembly,
                language: language,
                filePath: projectPath,
                outputFilePath: string.Empty,
                outputRefFilePath: string.Empty,
                compilationOptions: null, //compilationOptions,
                parseOptions: null, //parseOptions,
                documents: Enumerable.Empty<DocumentInfo>(),
                projectReferences: Enumerable.Empty<ProjectReference>(),
                metadataReferences: Enumerable.Empty<MetadataReference>(),
                analyzerReferences: Enumerable.Empty<AnalyzerReference>(),
                additionalDocuments: Enumerable.Empty<DocumentInfo>(),
                isSubmission: false,
                hostObjectType: null);
        }

        var isCSharp = language == LanguageNames.CSharp;
        static CommandLineParser GetCommandLineParser(bool isCSharp) =>
            isCSharp ? CSharpDefaults.CommandLineParser : VisualBasicDefaults.CommandLineParser;

        var commandLineParser = GetCommandLineParser(isCSharp);
        var commandLineArgs = commandLineParser.Parse(
            args: projectFileInfo.CommandLineArgs,
            baseDirectory: projectDirectory,
            sdkDirectory: RuntimeEnvironment.GetRuntimeDirectory(),
            additionalReferenceDirectories: null);

        var assemblyName = commandLineArgs.CompilationName;
        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            assemblyName = GetAssemblyNameFromProjectPath(projectPath);
        }

        var parseOptions = commandLineArgs.ParseOptions;
        if (parseOptions.DocumentationMode == DocumentationMode.None)
        {
            parseOptions = parseOptions.WithDocumentationMode(DocumentationMode.Parse);
        }

        static CompilationOptions GetCompilationOptions(bool isCSharp) =>
            isCSharp ? CSharpDefaults.CompilationOptions : VisualBasicDefaults.CompilationOptions;

        //// add all the extra options that are really behavior overrides
        // var metadataService = GetWorkspaceService<IMetadataService>();
        var compilationOptions = GetCompilationOptions(isCSharp)
            .WithXmlReferenceResolver(new XmlFileResolver(projectDirectory))
            .WithSourceReferenceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, projectDirectory))
            //    // TODO: https://github.com/dotnet/roslyn/issues/4967
            //    .WithMetadataReferenceResolver(new WorkspaceMetadataFileReferenceResolver(metadataService, new RelativePathResolver(ImmutableArray<string>.Empty, projectDirectory)))
            .WithStrongNameProvider(new DesktopStrongNameProvider(commandLineArgs.KeyFileSearchPaths))
            .WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default);

        var documents = CreateDocumentInfos(projectFileInfo.Documents, projectId, commandLineArgs.Encoding);
        var additionalDocuments = CreateDocumentInfos(projectFileInfo.AdditionalDocuments, projectId, commandLineArgs.Encoding);
        var analyzerConfigDocuments = CreateDocumentInfos(projectFileInfo.AnalyzerConfigDocuments, projectId, commandLineArgs.Encoding);
        CheckForDuplicateDocuments(documents.Concat(additionalDocuments).Concat(analyzerConfigDocuments).ToImmutableArray());
        //
        //var analyzerReferences = ResolveAnalyzerReferences(commandLineArgs);
        //var resolvedReferences = await ResolveReferencesAsync(projectId, projectFileInfo, commandLineArgs, cancellationToken).ConfigureAwait(false);

        await Task.CompletedTask;

        return ProjectInfo.Create(
            projectId,
            version,
            projectName,
            assemblyName,
            language,
            projectPath,
            outputFilePath: projectFileInfo.OutputFilePath,
            outputRefFilePath: projectFileInfo.OutputRefFilePath,
            compilationOptions: compilationOptions,
            parseOptions: parseOptions,
            documents: documents,
            projectReferences: null, //resolvedReferences.ProjectReferences,
            metadataReferences: null, //resolvedReferences.MetadataReferences,
            analyzerReferences: null, //analyzerReferences,
            additionalDocuments: null, //additionalDocuments,
            isSubmission: false,
            hostObjectType: null)
            .WithDefaultNamespace(projectFileInfo.DefaultNamespace)
            .WithAnalyzerConfigDocuments(analyzerConfigDocuments);
    }

    async Task<ProjectFileInfo> BuildProjectFileInfoAsync(
        MSBProject loadedProject, string language, string projectDirectory, CancellationToken cancellationToken)
    {
        var project =
            await BuildProjectAsync(loadedProject, cancellationToken).ConfigureAwait(false);

        return project != null
            ? CreateProjectFileInfo(project, loadedProject, language, projectDirectory)
            : ProjectFileInfo.CreateEmpty(language, loadedProject.FullPath);
    }

    async Task<MSBProjectIntance> BuildProjectAsync(
        MSBProject project, CancellationToken cancellationToken)
    {
        var projectInstance = project.CreateProjectInstance();

        var targets = new[] { "Compile", "CoreCompile" };
        foreach (var target in targets)
        {
            if (!projectInstance.Targets.ContainsKey(target))
            {
                return projectInstance;
            }
        }

        var buildRequestData = new BuildRequestData(projectInstance, targets);

        var result = await BuildAsync(buildRequestData, cancellationToken).ConfigureAwait(false);
        if (result.OverallResult == BuildResultCode.Failure)
        {
            if (result.Exception != null)
            {
                _logger.LogError(projectInstance.FullPath, result.Exception);
            }
        }

        return projectInstance;
    }

    Task<BuildResult> BuildAsync(BuildRequestData requestData, CancellationToken cancellationToken)
    {
        TaskCompletionSource<BuildResult> taskSource = new();
        CancellationTokenRegistration registration = default;

        if (cancellationToken.CanBeCanceled)
        {
            registration = cancellationToken.Register(() =>
            {
                taskSource.TrySetCanceled();
                _buildManager.CancelAllSubmissions();
                registration.Dispose();
            });
        }

        try
        {
            _buildManager.PendBuildRequest(requestData).ExecuteAsync(sub =>
            {
                try
                {
                    var result = sub.BuildResult;
                    registration.Dispose();
                    taskSource.TrySetResult(result);
                }
                catch (Exception e)
                {
                    taskSource.TrySetException(e);
                }
            }, null);
        }
        catch (Exception e)
        {
            taskSource.SetException(e);
        }

        return taskSource.Task;
    }

    ProjectFileInfo CreateProjectFileInfo(
        ProjectInstance projectInstance, MSBProject loadedProject, string language, string projectDirectory)
    {
        var commandLineArgs = GetCommandLineArgs(projectInstance, language);

        var outputFilePath = projectInstance.GetPropertyValue("TargetPath");
        if (!string.IsNullOrWhiteSpace(outputFilePath))
        {
            outputFilePath = GetAbsolutePathRelativeToProject(outputFilePath, projectDirectory);
        }

        var outputRefFilePath = projectInstance.GetPropertyValue("TargetRefPath");
        if (!string.IsNullOrWhiteSpace(outputRefFilePath))
        {
            outputRefFilePath = GetAbsolutePathRelativeToProject(outputRefFilePath, projectDirectory);
        }

        var defaultNamespace = projectInstance.GetPropertyValue("RootNamespace") ?? string.Empty;
        var targetFramework = projectInstance.GetPropertyValue("TargetFramework");
        if (string.IsNullOrWhiteSpace(targetFramework))
        {
            targetFramework = null;
        }

        var docs = projectInstance.GetItems("Compile")
            .Cast<ITaskItem>()
            .Where(i => !Path.GetFileName(i.ItemSpec).StartsWith("TemporaryGeneratedFile_", StringComparison.Ordinal))
            .Select(i => MakeDocumentFileInfo(loadedProject, i, projectDirectory))
            .ToImmutableArray();

        var additionalDocs = projectInstance.GetItems("AdditionalFiles")
            .Cast<ITaskItem>()
            .Select(i => MakeDocumentFileInfo(loadedProject, i, projectDirectory))
            .ToImmutableArray();

        //var analyzerConfigDocs = project.GetEditorConfigFiles()
        //    .Select(MakeNonSourceFileDocumentFileInfo)
        //    .ToImmutableArray();

        return ProjectFileInfo.Create(
            language,
            projectInstance.FullPath,
            outputFilePath,
            outputRefFilePath,
            defaultNamespace,
            targetFramework ?? "<unknown>",
            commandLineArgs,
            docs,
            additionalDocs,
            ImmutableArray<DocumentFileInfo>.Empty, //analyzerConfigDocs,
            projectInstance.GetItems("ProjectReference").Where(i =>
            {
                var referenceOutputAssemblyText = i.GetMetadataValue("ReferenceOutputAssembly");

                return !string.IsNullOrWhiteSpace(referenceOutputAssemblyText)
                    ? !string.Equals(referenceOutputAssemblyText, bool.FalseString, StringComparison.OrdinalIgnoreCase)
                    : true;
            })
            .Select(i =>
            {
                var aliasesText = i.GetMetadataValue("Aliases");
                var aliases = !string.IsNullOrWhiteSpace(aliasesText)
                    ? ImmutableArray.CreateRange(aliasesText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()))
                    : ImmutableArray<string>.Empty;

                return new ProjectFileReference(i.EvaluatedInclude, aliases);
            }).ToImmutableArray());
    }

    ImmutableArray<string> GetCommandLineArgs(ProjectInstance project, string language)
    {
        var isCSharp = language == LanguageNames.CSharp;
        var commandLineArgs = project.GetItems(isCSharp ? "CscCommandLineArgs" : "VbcCommandLineArgs")
            .Cast<ITaskItem>()
            .Select(item => item.ItemSpec)
            .ToImmutableArray();

        return commandLineArgs;
    }

    string GetAbsolutePathRelativeToProject(string path, string projectDirectory)
    {
        var absolutePath = FileUtilities.ResolveRelativePath(path, projectDirectory) ?? path;
        return FileUtilities.TryNormalizeAbsolutePath(absolutePath) ?? absolutePath;
    }

    DocumentFileInfo MakeDocumentFileInfo(
        MSBProject project, ITaskItem documentItem, string projectDirectory)
    {
        var filePath = GetAbsolutePathRelativeToProject(documentItem.ItemSpec, projectDirectory);
        var logicalPath = GetDocumentLogicalPath(documentItem, projectDirectory);
        var isLinked = !string.IsNullOrEmpty(documentItem.GetMetadata("Link"));
        var isGenerated = IsDocumentGenerated(project, documentItem, projectDirectory);

        return new DocumentFileInfo(filePath, logicalPath, isLinked, isGenerated, SourceCodeKind.Regular);
    }

    string GetDocumentLogicalPath(ITaskItem documentItem, string projectDirectory)
    {
        var link = documentItem.GetMetadata("Link");
        if (!string.IsNullOrEmpty(link))
        {
            // if a specific link is specified in the project file then use it to form the logical path.
            return link;
        }
        else
        {
            var filePath = documentItem.ItemSpec;
            if (!PathUtilities.IsAbsolute(filePath))
            {
                return filePath;
            }

            var normalizedPath = FileUtilities.TryNormalizeAbsolutePath(filePath);
            if (normalizedPath == null)
            {
                return filePath;
            }

            // If the document is within the current project directory (or subdirectory), then the logical path is the relative path 
            // from the project's directory.
            if (normalizedPath.StartsWith(projectDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return normalizedPath.Substring(projectDirectory.Length);
            }
            else
            {
                // if the document lies outside the project's directory (or subdirectory) then place it logically at the root of the project.
                // if more than one document ends up with the same logical name then so be it (the workspace will survive.)
                return PathUtilities.GetFileName(normalizedPath);
            }
        }
    }

    bool IsDocumentGenerated(MSBProject project, ITaskItem documentItem, string projectDirectory)
    {
        foreach (var item in project.GetItems("Compile"))
        {
            _documents[GetAbsolutePathRelativeToProject(item.EvaluatedInclude, projectDirectory)] = item;
        }

        return !_documents.ContainsKey(
            GetAbsolutePathRelativeToProject(documentItem.ItemSpec, projectDirectory));
    }

    static string GetAssemblyNameFromProjectPath(string projectFilePath)
    {
        var assemblyName = Path.GetFileNameWithoutExtension(projectFilePath);
        return string.IsNullOrWhiteSpace(assemblyName) ? "assembly" : assemblyName;
    }

    static ImmutableArray<DocumentInfo> CreateDocumentInfos(
        IReadOnlyList<DocumentFileInfo> documentFileInfos, ProjectId projectId, Encoding? encoding)
    {
        var results = ImmutableArray.CreateBuilder<DocumentInfo>();

        foreach (var info in documentFileInfos)
        {
            GetDocumentNameAndFolders(info.LogicalPath, out var name, out var folders);

            var documentInfo = DocumentInfo.Create(
                DocumentId.CreateNewId(projectId, debugName: info.FilePath),
                name,
                folders,
                info.SourceCodeKind,
                new FileTextLoader(info.FilePath, encoding),
                info.FilePath,
                info.IsGenerated);

            results.Add(documentInfo);
        }

        return results.ToImmutable();
    }

    static void GetDocumentNameAndFolders(
        string logicalPath, out string name, out ImmutableArray<string> folders)
    {
        var pathNames = logicalPath.Split(s_directorySplitChars, StringSplitOptions.RemoveEmptyEntries);
        if (pathNames.Length > 0)
        {
            if (pathNames.Length > 1)
            {
                folders = pathNames.Take(pathNames.Length - 1).ToImmutableArray();
            }
            else
            {
                folders = ImmutableArray<string>.Empty;
            }

            name = pathNames[pathNames.Length - 1];
        }
        else
        {
            name = logicalPath;
            folders = ImmutableArray<string>.Empty;
        }
    }

    void CheckForDuplicateDocuments(ImmutableArray<DocumentInfo> documents)
    {
        var paths = new HashSet<string>(PathUtilities.Comparer);
        foreach (var doc in documents)
        {
            if (doc.FilePath is null)
            {
                continue;
            }

            if (paths.Contains(doc.FilePath))
            {
                _logger.LogInformation($"Duplicate source file: {doc.FilePath}");
            }

            paths.Add(doc.FilePath);
        }
    }

    TLanguageService? GetLanguageService<TLanguageService>(string languageName)
        where TLanguageService : ILanguageService =>
        _workspaceServices.GetLanguageServices(languageName)
            .GetService<TLanguageService>();

    TWorkspaceService? GetWorkspaceService<TWorkspaceService>()
        where TWorkspaceService : IWorkspaceService =>
        _workspaceServices.GetService<TWorkspaceService>();

    public void Dispose()
    {
        _buildManager?.Dispose();
        _buildManager = null!;
    }
}
