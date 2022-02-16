namespace DotNet.CodeAnalysis;

/// Borrowed and inspired by:
/// https://github.com/dotnet/roslyn/blob/main/src/Workspaces/Core/MSBuild/MSBuild/ProjectFile/DocumentFileInfo.cs
/// <summary>
/// Represents a source file that is part of a project file.
/// </summary>
internal sealed record DocumentFileInfo(
    /// <summary>
    /// The absolute path to the document file on disk.
    /// </summary>
    string FilePath,

    /// <summary>
    /// A fictional path to the document, relative to the project.
    /// The document may not actually exist at this location, and is used
    /// to represent linked documents. This includes the file name.
    /// </summary>
    string LogicalPath,

    /// <summary>
    /// True if the document has a logical path that differs from its 
    /// absolute file path.
    /// </summary>
    bool IsLinked,

    /// <summary>
    /// True if the file was generated during build.
    /// </summary>
    bool IsGenerated,

    /// <summary>
    /// The <see cref="SourceCodeKind"/> of this document.
    /// </summary>
    SourceCodeKind SourceCodeKind);
