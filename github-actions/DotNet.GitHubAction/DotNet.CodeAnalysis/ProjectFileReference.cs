using System.Collections.Immutable;

namespace DotNet.CodeAnalysis
{
    internal sealed record ProjectFileReference(
        /// <summary>
        /// The path on disk to the other project file. 
        /// This path may be relative to the referencing project's file or an absolute path.
        /// </summary>
        string Path,

        /// <summary>
        /// The aliases assigned to this reference, if any.
        /// </summary>
        ImmutableArray<string> Aliases);
}
