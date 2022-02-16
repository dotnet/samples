namespace DotNet.CodeAnalysis
{
    /// Internals borrowed from:
    /// https://github.com/dotnet/roslyn/blob/main/src/Workspaces/Core/MSBuild/MSBuild/ProjectFile/ProjectFileInfo.cs
    /// <summary>
    /// Provides information about a project that has been loaded from disk and
    /// built with MSBuild. If the project is multi-targeting, this represents
    /// the information from a single target framework.
    /// </summary>
    internal sealed class ProjectFileInfo
    {
        public bool IsEmpty { get; }

        /// <summary>
        /// The language of this project.
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// The path to the project file for this project.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The path to the output file this project generates.
        /// </summary>
        public string OutputFilePath { get; }

        /// <summary>
        /// The path to the reference assembly output file this project generates.
        /// </summary>
        public string OutputRefFilePath { get; }

        /// <summary>
        /// The default namespace of the project ("" if not defined, which means global namespace),
        /// or null if it is unknown or not applicable. 
        /// </summary>
        /// <remarks>
        /// Right now VB doesn't have the concept of "default namespace". But we conjure one in workspace 
        /// by assigning the value of the project's root namespace to it. So various feature can choose to 
        /// use it for their own purpose.
        /// In the future, we might consider officially exposing "default namespace" for VB project 
        /// (e.g. through a "defaultnamespace" msbuild property)
        /// </remarks>
        public string DefaultNamespace { get; }

        /// <summary>
        /// The target framework of this project.
        /// This takes the form of the 'short name' form used by NuGet (e.g. net46, netcoreapp2.0, etc.)
        /// </summary>
        public string TargetFramework { get; }

        /// <summary>
        /// The command line args used to compile the project.
        /// </summary>
        public ImmutableArray<string> CommandLineArgs { get; }

        /// <summary>
        /// The source documents.
        /// </summary>
        public ImmutableArray<DocumentFileInfo> Documents { get; }

        /// <summary>
        /// The additional documents.
        /// </summary>
        public ImmutableArray<DocumentFileInfo> AdditionalDocuments { get; }

        /// <summary>
        /// The analyzer config documents.
        /// </summary>
        public ImmutableArray<DocumentFileInfo> AnalyzerConfigDocuments { get; }

        /// <summary>
        /// References to other projects.
        /// </summary>
        public ImmutableArray<ProjectFileReference> ProjectReferences { get; }

        public override string ToString() =>
            string.IsNullOrWhiteSpace(TargetFramework)
                ? FilePath
                : $"{FilePath} ({TargetFramework})";

        private ProjectFileInfo(
            bool isEmpty,
            string language,
            string filePath,
            string outputFilePath,
            string outputRefFilePath,
            string defaultNamespace,
            string targetFramework,
            ImmutableArray<string> commandLineArgs,
            ImmutableArray<DocumentFileInfo> documents,
            ImmutableArray<DocumentFileInfo> additionalDocuments,
            ImmutableArray<DocumentFileInfo> analyzerConfigDocuments,
            ImmutableArray<ProjectFileReference> projectReferences)
        {
            IsEmpty = isEmpty;
            Language = language;
            FilePath = filePath;
            OutputFilePath = outputFilePath;
            OutputRefFilePath = outputRefFilePath;
            DefaultNamespace = defaultNamespace;
            TargetFramework = targetFramework;
            CommandLineArgs = commandLineArgs;
            Documents = documents;
            AdditionalDocuments = additionalDocuments;
            AnalyzerConfigDocuments = analyzerConfigDocuments;
            ProjectReferences = projectReferences;
        }

        public static ProjectFileInfo Create(
            string language,
            string filePath,
            string outputFilePath,
            string outputRefFilePath,
            string defaultNamespace,
            string targetFramework,
            ImmutableArray<string> commandLineArgs,
            ImmutableArray<DocumentFileInfo> documents,
            ImmutableArray<DocumentFileInfo> additionalDocuments,
            ImmutableArray<DocumentFileInfo> analyzerConfigDocuments,
            ImmutableArray<ProjectFileReference> projectReferences)
            => new(
                isEmpty: false,
                language,
                filePath,
                outputFilePath,
                outputRefFilePath,
                defaultNamespace,
                targetFramework,
                commandLineArgs,
                documents,
                additionalDocuments,
                analyzerConfigDocuments,
                projectReferences);

        public static ProjectFileInfo CreateEmpty(string language, string filePath)
            => new(
                isEmpty: true,
                language,
                filePath,
                outputFilePath: null!,
                outputRefFilePath: null!,
                defaultNamespace: null!,
                targetFramework: null!,
                commandLineArgs: ImmutableArray<string>.Empty,
                documents: ImmutableArray<DocumentFileInfo>.Empty,
                additionalDocuments: ImmutableArray<DocumentFileInfo>.Empty,
                analyzerConfigDocuments: ImmutableArray<DocumentFileInfo>.Empty,
                projectReferences: ImmutableArray<ProjectFileReference>.Empty);
    }
}
