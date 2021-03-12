namespace DotNet.CodeAnalysis
{
    /// Internals borrowed from:
    /// https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/FileSystem/PathKind.cs
    internal enum PathKind
    {
        /// <summary>
        /// Null or empty.
        /// </summary>
        Empty,

        /// <summary>
        /// "file"
        /// </summary>
        Relative,

        /// <summary>
        /// ".\file"
        /// </summary>
        RelativeToCurrentDirectory,

        /// <summary>
        /// "..\file"
        /// </summary>
        RelativeToCurrentParent,

        /// <summary>
        /// "\dir\file"
        /// </summary>
        RelativeToCurrentRoot,

        /// <summary>
        /// "C:dir\file"
        /// </summary>
        RelativeToDriveDirectory,

        /// <summary>
        /// "C:\file" or "\\machine" (UNC).
        /// </summary>
        Absolute,
    }
}
