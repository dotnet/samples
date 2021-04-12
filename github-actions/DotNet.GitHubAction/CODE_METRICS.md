<!-- markdownlint-capture -->
<!-- markdownlint-disable -->

# Code Metrics

This file is dynamically maintained by a bot, *please do not* edit this by hand. It represents various [code metrics](https://aka.ms/dotnet/code-metrics), such as cyclomatic complexity, maintainability index, and so on.

<div id='dotnet-codeanalysis'></div>

## DotNet.CodeAnalysis :feelsgood:

The *DotNet.CodeAnalysis.csproj* project file contains:

- 3 namespaces.
- 16 named types.
- 2,593 total lines of source code.
- Approximately 643 lines of executable code.
- The highest cyclomatic complexity is 17 :feelsgood:.

<details>
<summary>
  <strong id="dotnet-codeanalysis">
    DotNet.CodeAnalysis :feelsgood:
  </strong>
</summary>
<br>

The `DotNet.CodeAnalysis` namespace contains 14 named types.

- 14 named types.
- 2,571 total lines of source code.
- Approximately 639 lines of executable code.
- The highest cyclomatic complexity is 17 :feelsgood:.

<details>
<summary>
  <strong id="documentfileinfo">
    DocumentFileInfo :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DocumentFileInfo` contains 6 members.
- 33 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [10](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/DocumentFileInfo.cs#L10 "DocumentFileInfo.DocumentFileInfo(string FilePath, string LogicalPath, bool IsLinked, bool IsGenerated, SourceCodeKind SourceCodeKind)") | 100 | 1 :heavy_check_mark: | 0 | 1 | 33 / 0 |
| Property | [14](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/DocumentFileInfo.cs#L14 "string DocumentFileInfo.FilePath") | 100 | 0 :heavy_check_mark: | 0 | 0 | 3 / 0 |
| Property | [32](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/DocumentFileInfo.cs#L32 "bool DocumentFileInfo.IsGenerated") | 100 | 0 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [27](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/DocumentFileInfo.cs#L27 "bool DocumentFileInfo.IsLinked") | 100 | 0 :heavy_check_mark: | 0 | 0 | 5 / 0 |
| Property | [21](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/DocumentFileInfo.cs#L21 "string DocumentFileInfo.LogicalPath") | 100 | 0 :heavy_check_mark: | 0 | 0 | 6 / 0 |
| Property | [37](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/DocumentFileInfo.cs#L37 "SourceCodeKind DocumentFileInfo.SourceCodeKind") | 100 | 0 :heavy_check_mark: | 0 | 1 | 4 / 0 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="filenameutilities">
    FileNameUtilities :warning:
  </strong>
</summary>
<br>

- The `FileNameUtilities` contains 10 members.
- 183 total lines of source code.
- Approximately 51 lines of executable code.
- The highest cyclomatic complexity is 8 :warning:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [17](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L17 "char FileNameUtilities.AltDirectorySeparatorChar") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [132](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L132 "string? FileNameUtilities.ChangeExtension(string? path, string? extension)") | 63 | 6 :heavy_check_mark: | 0 | 3 | 31 / 9 |
| Field | [16](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L16 "char FileNameUtilities.DirectorySeparatorChar") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [80](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L80 "string? FileNameUtilities.GetExtension(string? path)") | 72 | 3 :heavy_check_mark: | 0 | 3 | 18 / 5 |
| Method | [181](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L181 "string? FileNameUtilities.GetFileName(string? path, bool includeExtension = true)") | 70 | 3 :heavy_check_mark: | 0 | 3 | 11 / 5 |
| Method | [40](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L40 "int FileNameUtilities.IndexOfExtension(string? path)") | 58 | 8 :warning: | 0 | 2 | 38 / 13 |
| Method | [157](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L157 "int FileNameUtilities.IndexOfFileName(string? path)") | 65 | 6 :heavy_check_mark: | 0 | 2 | 22 / 7 |
| Method | [28](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L28 "bool FileNameUtilities.IsFileName(string? path)") | 92 | 1 :heavy_check_mark: | 0 | 2 | 12 / 1 |
| Method | [99](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L99 "string? FileNameUtilities.RemoveExtension(string? path)") | 63 | 5 :heavy_check_mark: | 0 | 3 | 29 / 9 |
| Field | [18](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileNameUtilities.cs#L18 "char FileNameUtilities.VolumeSeparatorChar") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="fileutilities">
    FileUtilities :feelsgood:
  </strong>
</summary>
<br>

- The `FileUtilities` contains 17 members.
- 392 total lines of source code.
- Approximately 82 lines of executable code.
- The highest cyclomatic complexity is 17 :feelsgood:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [302](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L302 "Stream FileUtilities.CreateFileStreamChecked(Func<string, Stream> factory, string path, string? paramName = null)") | 66 | 3 :heavy_check_mark: | 0 | 9 | 41 / 8 |
| Method | [172](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L172 "string? FileUtilities.GetBaseDirectory(string? basePath, string? baseDirectory)") | 72 | 2 :heavy_check_mark: | 0 | 2 | 19 / 6 |
| Method | [358](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L358 "long FileUtilities.GetFileLength(string fullPath)") | 79 | 1 :heavy_check_mark: | 0 | 5 | 17 / 4 |
| Method | [341](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L341 "DateTime FileUtilities.GetFileTimeStamp(string fullPath)") | 84 | 1 :heavy_check_mark: | 0 | 5 | 16 / 3 |
| Method | [223](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L223 "string FileUtilities.NormalizeAbsolutePath(string path)") | 75 | 1 :heavy_check_mark: | 0 | 6 | 26 / 5 |
| Method | [244](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L244 "string FileUtilities.NormalizeDirectoryPath(string path)") | 96 | 1 :heavy_check_mark: | 0 | 1 | 4 / 1 |
| Method | [194](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L194 "string? FileUtilities.NormalizeRelativePath(string path, string? basePath, string? baseDirectory)") | 64 | 5 :heavy_check_mark: | 0 | 2 | 22 / 9 |
| Method | [277](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L277 "Stream FileUtilities.OpenAsyncRead(string fullPath)") | 90 | 1 :heavy_check_mark: | 0 | 3 | 4 / 2 |
| Method | [375](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L375 "Stream FileUtilities.OpenFileStream(string path)") | 78 | 1 :heavy_check_mark: | 0 | 7 | 23 / 4 |
| Method | [261](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L261 "Stream FileUtilities.OpenRead(string fullPath)") | 84 | 1 :heavy_check_mark: | 0 | 6 | 15 / 3 |
| Method | [37](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L37 "string? FileUtilities.ResolveRelativePath(string path, string? basePath, string? baseDirectory, IEnumerable<string> searchPaths, Func<string, bool> fileExists)") | 56 | 8 :warning: | 0 | 6 | 72 / 17 |
| Method | [84](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L84 "string? FileUtilities.ResolveRelativePath(string? path, string? baseDirectory)") | 93 | 1 :heavy_check_mark: | 0 | 1 | 2 / 1 |
| Method | [87](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L87 "string? FileUtilities.ResolveRelativePath(string? path, string? basePath, string? baseDirectory)") | 91 | 1 :heavy_check_mark: | 0 | 3 | 4 / 1 |
| Method | [92](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L92 "string? FileUtilities.ResolveRelativePath(PathKind kind, string? path, string? basePath, string? baseDirectory)") | 57 | 17 :feelsgood: | 0 | 6 | 79 / 11 |
| Method | [282](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L282 "T FileUtilities.RethrowExceptionsAsIOException<T>(Func<T> operation)") | 84 | 1 :heavy_check_mark: | 0 | 5 | 15 / 3 |
| Field | [192](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L192 "char[] FileUtilities.s_invalidPathChars") | 93 | 0 :heavy_check_mark: | 0 | 1 | 1 / 1 |
| Method | [249](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/FileUtilities.cs#L249 "string? FileUtilities.TryNormalizeAbsolutePath(string path)") | 85 | 1 :heavy_check_mark: | 0 | 2 | 11 / 3 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="hash">
    Hash :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Hash` contains 23 members.
- 368 total lines of source code.
- Approximately 94 lines of executable code.
- The highest cyclomatic complexity is 5 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [15](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L15 "int Hash.Combine(int newKey, int currentKey)") | 91 | 1 :heavy_check_mark: | 0 | 1 | 7 / 1 |
| Method | [20](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L20 "int Hash.Combine(bool newKeyPart, int currentKey)") | 91 | 2 :heavy_check_mark: | 0 | 0 | 4 / 1 |
| Method | [31](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L31 "int Hash.Combine<T>(T newKeyPart, int currentKey)") | 75 | 2 :heavy_check_mark: | 0 | 1 | 17 / 4 |
| Method | [354](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L354 "int Hash.CombineFNVHash(int hashCode, string text)") | 79 | 2 :heavy_check_mark: | 0 | 1 | 16 / 3 |
| Method | [371](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L371 "int Hash.CombineFNVHash(int hashCode, char ch)") | 91 | 1 :heavy_check_mark: | 0 | 1 | 11 / 1 |
| Method | [43](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L43 "int Hash.CombineValues<T>(IEnumerable<T>? values, int maxItemsToHash = null)") | 62 | 5 :heavy_check_mark: | 0 | 4 | 25 / 11 |
| Method | [69](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L69 "int Hash.CombineValues<T>(T[]? values, int maxItemsToHash = null)") | 62 | 4 :heavy_check_mark: | 0 | 1 | 23 / 10 |
| Method | [93](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L93 "int Hash.CombineValues<T>(ImmutableArray<T> values, int maxItemsToHash = null)") | 63 | 5 :heavy_check_mark: | 0 | 3 | 25 / 11 |
| Method | [119](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L119 "int Hash.CombineValues(IEnumerable<string?>? values, StringComparer stringComparer, int maxItemsToHash = null)") | 62 | 5 :heavy_check_mark: | 0 | 5 | 24 / 11 |
| Field | [148](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L148 "int Hash.FnvOffsetBias") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Field | [154](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L154 "int Hash.FnvPrime") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [253](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L253 "int Hash.GetCaseInsensitiveFNVHashCode(string text)") | 96 | 1 :heavy_check_mark: | 0 | 1 | 4 / 1 |
| Method | [258](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L258 "int Hash.GetCaseInsensitiveFNVHashCode(ReadOnlySpan<char> data)") | 73 | 2 :heavy_check_mark: | 0 | 2 | 11 / 4 |
| Method | [162](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L162 "int Hash.GetFNVHashCode(byte[] data)") | 73 | 2 :heavy_check_mark: | 0 | 1 | 17 / 4 |
| Method | [183](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L183 "int Hash.GetFNVHashCode(ReadOnlySpan<byte> data, out bool isAscii)") | 64 | 2 :heavy_check_mark: | 0 | 2 | 25 / 8 |
| Method | [206](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L206 "int Hash.GetFNVHashCode(ImmutableArray<byte> data)") | 73 | 2 :heavy_check_mark: | 0 | 2 | 17 / 4 |
| Method | [226](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L226 "int Hash.GetFNVHashCode(ReadOnlySpan<char> data)") | 73 | 2 :heavy_check_mark: | 0 | 2 | 19 / 4 |
| Method | [250](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L250 "int Hash.GetFNVHashCode(string text, int start, int length)") | 95 | 1 :heavy_check_mark: | 0 | 1 | 14 / 1 |
| Method | [277](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L277 "int Hash.GetFNVHashCode(string text, int start)") | 93 | 1 :heavy_check_mark: | 0 | 1 | 11 / 1 |
| Method | [288](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L288 "int Hash.GetFNVHashCode(string text)") | 93 | 1 :heavy_check_mark: | 0 | 0 | 10 / 1 |
| Method | [299](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L299 "int Hash.GetFNVHashCode(StringBuilder text)") | 71 | 2 :heavy_check_mark: | 0 | 4 | 18 / 5 |
| Method | [320](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L320 "int Hash.GetFNVHashCode(char[] text, int start, int length)") | 70 | 2 :heavy_check_mark: | 0 | 1 | 20 / 5 |
| Method | [342](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/Hash.cs#L342 "int Hash.GetFNVHashCode(char ch)") | 93 | 1 :heavy_check_mark: | 0 | 0 | 13 / 1 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="pathutilities-pathcomparer">
    PathUtilities.PathComparer :heavy_check_mark:
  </strong>
</summary>
<br>

- The `PathUtilities.PathComparer` contains 2 members.
- 22 total lines of source code.
- Approximately 6 lines of executable code.
- The highest cyclomatic complexity is 5 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [729](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L729 "bool PathComparer.Equals(string? x, string? y)") | 71 | 5 :heavy_check_mark: | 0 | 1 | 14 / 5 |
| Method | [744](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L744 "int PathComparer.GetHashCode(string? s)") | 96 | 1 :heavy_check_mark: | 0 | 1 | 4 / 1 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="pathkind">
    PathKind :heavy_check_mark:
  </strong>
</summary>
<br>

- The `PathKind` contains 7 members.
- 39 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [40](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathKind.cs#L40 "PathKind.Absolute") | 100 | 0 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Field | [10](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathKind.cs#L10 "PathKind.Empty") | 100 | 0 :heavy_check_mark: | 0 | 0 | 3 / 0 |
| Field | [15](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathKind.cs#L15 "PathKind.Relative") | 100 | 0 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Field | [20](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathKind.cs#L20 "PathKind.RelativeToCurrentDirectory") | 100 | 0 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Field | [25](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathKind.cs#L25 "PathKind.RelativeToCurrentParent") | 100 | 0 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Field | [30](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathKind.cs#L30 "PathKind.RelativeToCurrentRoot") | 100 | 0 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Field | [35](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathKind.cs#L35 "PathKind.RelativeToDriveDirectory") | 100 | 0 :heavy_check_mark: | 0 | 0 | 4 / 0 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="pathutilities">
    PathUtilities :feelsgood:
  </strong>
</summary>
<br>

- The `PathUtilities` contains 45 members.
- 745 total lines of source code.
- Approximately 198 lines of executable code.
- The highest cyclomatic complexity is 17 :feelsgood:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [21](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L21 "char PathUtilities.AltDirectorySeparatorChar") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [94](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L94 "string PathUtilities.ChangeExtension(string path, string? extension)") | 97 | 1 :heavy_check_mark: | 0 | 1 | 4 / 1 |
| Method | [388](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L388 "string? PathUtilities.CombineAbsoluteAndRelativePaths(string root, string relativePath)") | 97 | 1 :heavy_check_mark: | 0 | 1 | 16 / 1 |
| Method | [415](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L415 "string PathUtilities.CombinePathsUnchecked(string root, string? relativePath)") | 72 | 3 :heavy_check_mark: | 0 | 2 | 10 / 4 |
| Method | [400](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L400 "string? PathUtilities.CombinePossiblyRelativeAndRelativePaths(string? root, string? relativePath)") | 78 | 2 :heavy_check_mark: | 0 | 3 | 21 / 3 |
| Field | [725](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L725 "IEqualityComparer<string> PathUtilities.Comparer") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Method | [262](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L262 "int PathUtilities.ConsumeDirectorySeparators(string path, int length, int i)") | 79 | 3 :heavy_check_mark: | 0 | 1 | 9 / 3 |
| Method | [462](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L462 "bool PathUtilities.ContainsPathComponent(string? path, string component, bool ignoreCase)") | 60 | 7 :heavy_check_mark: | 0 | 2 | 35 / 12 |
| Field | [20](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L20 "char PathUtilities.DirectorySeparatorChar") | 88 | 1 :heavy_check_mark: | 0 | 1 | 1 / 1 |
| Field | [24](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L24 "string PathUtilities.DirectorySeparatorStr") | 90 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [64](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L64 "string PathUtilities.EnsureTrailingSeparator(string s)") | 62 | 7 :heavy_check_mark: | 0 | 1 | 27 / 9 |
| Method | [117](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L117 "string? PathUtilities.GetDirectoryName(string? path)") | 93 | 1 :heavy_check_mark: | 0 | 1 | 11 / 1 |
| Method | [122](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L122 "string? PathUtilities.GetDirectoryName(string? path, bool isUnixLike)") | 60 | 7 :heavy_check_mark: | 0 | 2 | 28 / 12 |
| Method | [89](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L89 "string PathUtilities.GetExtension(string path)") | 100 | 1 :heavy_check_mark: | 0 | 1 | 4 / 1 |
| Method | [105](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L105 "string? PathUtilities.GetFileName(string? path, bool includeExtension = true)") | 78 | 1 :heavy_check_mark: | 0 | 3 | 5 / 3 |
| Method | [283](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L283 "PathKind PathUtilities.GetPathKind(string? path)") | 54 | 17 :feelsgood: | 0 | 3 | 58 / 16 |
| Method | [571](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L571 "string[] PathUtilities.GetPathParts(string path)") | 73 | 2 :heavy_check_mark: | 0 | 1 | 12 / 5 |
| Method | [174](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L174 "string? PathUtilities.GetPathRoot(string? path)") | 85 | 1 :heavy_check_mark: | 0 | 2 | 8 / 2 |
| Method | [180](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L180 "string? PathUtilities.GetPathRoot(string? path, bool isUnixLike)") | 73 | 3 :heavy_check_mark: | 0 | 2 | 17 / 6 |
| Method | [555](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L555 "string PathUtilities.GetRelativeChildPath(string parentPath, string childPath)") | 73 | 2 :heavy_check_mark: | 0 | 1 | 13 / 5 |
| Method | [490](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L490 "string PathUtilities.GetRelativePath(string directory, string fullPath)") | 52 | 10 :radioactive: | 0 | 1 | 56 / 20 |
| Method | [272](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L272 "string PathUtilities.GetUnixRoot(string path)") | 89 | 3 :heavy_check_mark: | 0 | 1 | 7 / 1 |
| Method | [197](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L197 "string PathUtilities.GetWindowsRoot(string path)") | 51 | 13 :feelsgood: | 0 | 1 | 64 / 21 |
| Method | [342](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L342 "bool PathUtilities.IsAbsolute(string? path)") | 67 | 6 :heavy_check_mark: | 0 | 3 | 28 / 7 |
| Method | [36](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L36 "bool PathUtilities.IsAnyDirectorySeparator(char c)") | 91 | 2 :heavy_check_mark: | 0 | 0 | 4 / 1 |
| Method | [547](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L547 "bool PathUtilities.IsChildPath(string parentPath, string childPath)") | 86 | 5 :heavy_check_mark: | 0 | 1 | 10 / 1 |
| Method | [31](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L31 "bool PathUtilities.IsDirectorySeparator(char c)") | 90 | 2 :heavy_check_mark: | 0 | 0 | 4 / 1 |
| Method | [371](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L371 "bool PathUtilities.IsDriveRootedAbsolutePath(string path)") | 88 | 3 :heavy_check_mark: | 0 | 1 | 7 / 1 |
| Method | [442](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L442 "bool PathUtilities.IsFilePath(string assemblyDisplayNameOrPath)") | 80 | 4 :heavy_check_mark: | 0 | 2 | 12 / 2 |
| Method | [151](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L151 "bool PathUtilities.IsSameDirectoryOrChildOf(string child, string parent)") | 67 | 3 :heavy_check_mark: | 0 | 2 | 18 / 8 |
| Property | [26](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L26 "bool PathUtilities.IsUnixLikePlatform") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 2 |
| Method | [690](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L690 "bool PathUtilities.IsValidFilePath(string? fullPath)") | 71 | 4 :heavy_check_mark: | 0 | 5 | 34 / 6 |
| Method | [643](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L643 "string PathUtilities.NormalizePathPrefix(string filePath, ImmutableArray<KeyValuePair<string, string>> pathMap)") | 58 | 10 :radioactive: | 0 | 4 | 35 / 13 |
| Method | [722](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L722 "string PathUtilities.NormalizeWithForwardSlash(string p)") | 91 | 2 :heavy_check_mark: | 0 | 1 | 10 / 1 |
| Field | [22](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L22 "string PathUtilities.ParentRelativeDirectory") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [613](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L613 "bool PathUtilities.PathCharEqual(char x, char y)") | 77 | 4 :heavy_check_mark: | 0 | 1 | 11 / 3 |
| Method | [625](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L625 "int PathUtilities.PathHashCode(string? path)") | 71 | 4 :heavy_check_mark: | 0 | 4 | 17 / 6 |
| Method | [587](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L587 "bool PathUtilities.PathsEqual(string path1, string path2)") | 95 | 1 :heavy_check_mark: | 0 | 1 | 7 / 1 |
| Method | [595](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L595 "bool PathUtilities.PathsEqual(string path1, string path2, int length)") | 69 | 5 :heavy_check_mark: | 0 | 1 | 20 / 6 |
| Method | [99](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L99 "string PathUtilities.RemoveExtension(string path)") | 97 | 1 :heavy_check_mark: | 0 | 1 | 4 / 1 |
| Method | [426](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L426 "string PathUtilities.RemoveTrailingDirectorySeparator(string path)") | 77 | 3 :heavy_check_mark: | 0 | 1 | 11 / 3 |
| Field | [569](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L569 "char[] PathUtilities.s_pathChars") | 86 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Field | [23](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L23 "string PathUtilities.ThisDirectory") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [45](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L45 "string PathUtilities.TrimTrailingSeparators(string s)") | 69 | 4 :heavy_check_mark: | 0 | 1 | 22 / 6 |
| Field | [25](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L25 "char PathUtilities.VolumeSeparatorChar") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="platforminformation">
    PlatformInformation :heavy_check_mark:
  </strong>
</summary>
<br>

- The `PlatformInformation` contains 3 members.
- 27 total lines of source code.
- Approximately 7 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [17](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PlatformInformation.cs#L17 "bool PlatformInformation.IsRunningOnMono") | 82 | 1 :heavy_check_mark: | 0 | 1 | 15 / 3 |
| Property | [16](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PlatformInformation.cs#L16 "bool PlatformInformation.IsUnix") | 98 | 2 :heavy_check_mark: | 0 | 1 | 1 / 2 |
| Property | [15](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PlatformInformation.cs#L15 "bool PlatformInformation.IsWindows") | 98 | 2 :heavy_check_mark: | 0 | 1 | 1 / 2 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="projectfileinfo">
    ProjectFileInfo :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ProjectFileInfo` contains 16 members.
- 149 total lines of source code.
- Approximately 15 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [85](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L85 "ProjectFileInfo.ProjectFileInfo(bool isEmpty, string language, string filePath, string outputFilePath, string outputRefFilePath, string defaultNamespace, string targetFramework, ImmutableArray<string> commandLineArgs, ImmutableArray<DocumentFileInfo> documents, ImmutableArray<DocumentFileInfo> additionalDocuments, ImmutableArray<DocumentFileInfo> analyzerConfigDocuments, ImmutableArray<ProjectFileReference> projectReferences)") | 60 | 1 :heavy_check_mark: | 0 | 3 | 27 / 12 |
| Property | [68](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L68 "ImmutableArray<DocumentFileInfo> ProjectFileInfo.AdditionalDocuments") | 100 | 1 :heavy_check_mark: | 0 | 2 | 4 / 0 |
| Property | [73](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L73 "ImmutableArray<DocumentFileInfo> ProjectFileInfo.AnalyzerConfigDocuments") | 100 | 1 :heavy_check_mark: | 0 | 2 | 4 / 0 |
| Property | [58](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L58 "ImmutableArray<string> ProjectFileInfo.CommandLineArgs") | 100 | 1 :heavy_check_mark: | 0 | 1 | 4 / 0 |
| Method | [113](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L113 "ProjectFileInfo ProjectFileInfo.Create(string language, string filePath, string outputFilePath, string outputRefFilePath, string defaultNamespace, string targetFramework, ImmutableArray<string> commandLineArgs, ImmutableArray<DocumentFileInfo> documents, ImmutableArray<DocumentFileInfo> additionalDocuments, ImmutableArray<DocumentFileInfo> analyzerConfigDocuments, ImmutableArray<ProjectFileReference> projectReferences)") | 88 | 1 :heavy_check_mark: | 0 | 3 | 25 / 1 |
| Method | [139](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L139 "ProjectFileInfo ProjectFileInfo.CreateEmpty(string language, string filePath)") | 92 | 1 :heavy_check_mark: | 0 | 1 | 14 / 1 |
| Property | [47](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L47 "string ProjectFileInfo.DefaultNamespace") | 100 | 1 :heavy_check_mark: | 0 | 0 | 12 / 0 |
| Property | [63](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L63 "ImmutableArray<DocumentFileInfo> ProjectFileInfo.Documents") | 100 | 1 :heavy_check_mark: | 0 | 2 | 4 / 0 |
| Property | [24](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L24 "string ProjectFileInfo.FilePath") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [14](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L14 "bool ProjectFileInfo.IsEmpty") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [19](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L19 "string ProjectFileInfo.Language") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [29](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L29 "string ProjectFileInfo.OutputFilePath") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [34](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L34 "string ProjectFileInfo.OutputRefFilePath") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [78](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L78 "ImmutableArray<ProjectFileReference> ProjectFileInfo.ProjectReferences") | 100 | 1 :heavy_check_mark: | 0 | 2 | 4 / 0 |
| Property | [53](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L53 "string ProjectFileInfo.TargetFramework") | 100 | 1 :heavy_check_mark: | 0 | 0 | 5 / 0 |
| Method | [80](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileInfo.cs#L80 "string ProjectFileInfo.ToString()") | 93 | 2 :heavy_check_mark: | 0 | 1 | 4 / 1 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="projectfilereference">
    ProjectFileReference :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ProjectFileReference` contains 3 members.
- 13 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [7](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileReference.cs#L7 "ProjectFileReference.ProjectFileReference(string Path, ImmutableArray<string> Aliases)") | 100 | 1 :heavy_check_mark: | 0 | 1 | 13 / 0 |
| Property | [17](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileReference.cs#L17 "ImmutableArray<string> ProjectFileReference.Aliases") | 100 | 0 :heavy_check_mark: | 0 | 1 | 4 / 0 |
| Property | [12](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectFileReference.cs#L12 "string ProjectFileReference.Path") | 100 | 0 :heavy_check_mark: | 0 | 0 | 4 / 0 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="projectloader">
    ProjectLoader :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ProjectLoader` contains 2 members.
- 24 total lines of source code.
- Approximately 7 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [18](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectLoader.cs#L18 "Project ProjectLoader.LoadProject(string path)") | 71 | 1 :heavy_check_mark: | 0 | 5 | 13 / 6 |
| Field | [12](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectLoader.cs#L12 "XmlReaderSettings ProjectLoader.s_xmlReaderSettings") | 93 | 0 :heavy_check_mark: | 0 | 2 | 4 / 1 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="projectworkspace">
    ProjectWorkspace :warning:
  </strong>
</summary>
<br>

- The `ProjectWorkspace` contains 28 members.
- 551 total lines of source code.
- Approximately 176 lines of executable code.
- The highest cyclomatic complexity is 8 :warning:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [29](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L29 "BuildManager ProjectWorkspace._buildManager") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Field | [35](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L35 "Dictionary<string, ProjectItem> ProjectWorkspace._documents") | 93 | 0 :heavy_check_mark: | 0 | 3 | 1 / 1 |
| Field | [34](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L34 "ILogger<ProjectWorkspace> ProjectWorkspace._logger") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [33](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L33 "ProjectLoader ProjectWorkspace._projectLoader") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [31](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L31 "AdhocWorkspace ProjectWorkspace._workspace") | 93 | 0 :heavy_check_mark: | 0 | 1 | 1 / 1 |
| Field | [32](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L32 "HostWorkspaceServices ProjectWorkspace._workspaceServices") | 93 | 0 :heavy_check_mark: | 0 | 1 | 1 / 1 |
| Method | [53](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L53 "ProjectWorkspace.ProjectWorkspace(ProjectLoader projectLoader, ILogger<ProjectWorkspace> logger)") | 89 | 1 :heavy_check_mark: | 0 | 6 | 3 / 1 |
| Method | [304](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L304 "Task<BuildResult> ProjectWorkspace.BuildAsync(BuildRequestData requestData, CancellationToken cancellationToken)") | 60 | 2 :heavy_check_mark: | 0 | 9 | 38 / 16 |
| Method | [276](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L276 "Task<ProjectInstance> ProjectWorkspace.BuildProjectAsync(Project project, CancellationToken cancellationToken)") | 61 | 5 :heavy_check_mark: | 0 | 8 | 27 / 11 |
| Method | [265](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L265 "Task<ProjectFileInfo> ProjectWorkspace.BuildProjectFileInfoAsync(Project loadedProject, string language, string projectDirectory, CancellationToken cancellationToken)") | 81 | 2 :heavy_check_mark: | 0 | 6 | 10 / 2 |
| Method | [542](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L542 "void ProjectWorkspace.CheckForDuplicateDocuments(ImmutableArray<DocumentInfo> documents)") | 70 | 4 :heavy_check_mark: | 0 | 8 | 18 / 7 |
| Method | [494](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L494 "ImmutableArray<DocumentInfo> ProjectWorkspace.CreateDocumentInfos(IReadOnlyList<DocumentFileInfo> documentFileInfos, ProjectId projectId, Encoding? encoding)") | 70 | 2 :heavy_check_mark: | 0 | 10 | 23 / 6 |
| Method | [343](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L343 "ProjectFileInfo ProjectWorkspace.CreateProjectFileInfo(ProjectInstance projectInstance, Project loadedProject, string language, string projectDirectory)") | 50 | 8 :warning: | 0 | 7 | 68 / 23 |
| Method | [157](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L157 "Task<ProjectInfo> ProjectWorkspace.CreateProjectInfoAsync(ProjectFileInfo projectFileInfo, string projectDirectory)") | 46 | 7 :heavy_check_mark: | 0 | 20 | 107 / 30 |
| Method | [570](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L570 "void ProjectWorkspace.Dispose()") | 87 | 2 :heavy_check_mark: | 0 | 2 | 5 / 2 |
| Method | [423](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L423 "string ProjectWorkspace.GetAbsolutePathRelativeToProject(string path, string projectDirectory)") | 85 | 3 :heavy_check_mark: | 0 | 2 | 5 / 2 |
| Method | [488](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L488 "string ProjectWorkspace.GetAssemblyNameFromProjectPath(string projectFilePath)") | 86 | 2 :heavy_check_mark: | 0 | 1 | 5 / 2 |
| Method | [412](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L412 "ImmutableArray<string> ProjectWorkspace.GetCommandLineArgs(ProjectInstance project, string language)") | 75 | 2 :heavy_check_mark: | 0 | 3 | 10 / 4 |
| Method | [440](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L440 "string ProjectWorkspace.GetDocumentLogicalPath(ITaskItem documentItem, string projectDirectory)") | 62 | 5 :heavy_check_mark: | 0 | 4 | 36 / 12 |
| Method | [518](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L518 "void ProjectWorkspace.GetDocumentNameAndFolders(string logicalPath, out string name, out ImmutableArray<string> folders)") | 66 | 3 :heavy_check_mark: | 0 | 2 | 23 / 8 |
| Method | [561](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L561 "TLanguageService? ProjectWorkspace.GetLanguageService<TLanguageService>(string languageName)") | 97 | 1 :heavy_check_mark: | 0 | 2 | 4 / 1 |
| Method | [566](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L566 "TWorkspaceService? ProjectWorkspace.GetWorkspaceService<TWorkspaceService>()") | 100 | 1 :heavy_check_mark: | 0 | 2 | 3 / 1 |
| Method | [477](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L477 "bool ProjectWorkspace.IsDocumentGenerated(Project project, ITaskItem documentItem, string projectDirectory)") | 78 | 2 :heavy_check_mark: | 0 | 6 | 10 / 3 |
| Method | [57](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L57 "Task<ImmutableArray<Project>> ProjectWorkspace.LoadProjectAsync(string path, CancellationToken cancellationToken)") | 56 | 3 :heavy_check_mark: | 0 | 18 | 45 / 17 |
| Method | [103](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L103 "Task<ImmutableArray<ProjectInfo>> ProjectWorkspace.LoadProjectInfosAsync(Project project, string language, string projectDirectory, CancellationToken cancellationToken)") | 53 | 6 :heavy_check_mark: | 0 | 8 | 53 / 21 |
| Method | [429](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L429 "DocumentFileInfo ProjectWorkspace.MakeDocumentFileInfo(Project project, ITaskItem documentItem, string projectDirectory)") | 71 | 1 :heavy_check_mark: | 0 | 4 | 10 / 5 |
| Field | [40](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L40 "ImmutableDictionary<string, string> ProjectWorkspace.s_defaultGlobalProperties") | 81 | 0 :heavy_check_mark: | 0 | 3 | 11 / 1 |
| Field | [37](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ProjectWorkspace.cs#L37 "char[] ProjectWorkspace.s_directorySplitChars") | 93 | 0 :heavy_check_mark: | 0 | 1 | 1 / 1 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="servicecollectionextensions">
    ServiceCollectionExtensions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ServiceCollectionExtensions` contains 1 members.
- 11 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [8](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/ServiceCollectionExtensions.cs#L8 "IServiceCollection ServiceCollectionExtensions.AddDotNetCodeAnalysisServices(IServiceCollection services)") | 93 | 1 :heavy_check_mark: | 0 | 2 | 8 / 2 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

<details>
<summary>
  <strong id="pathutilities-testaccessor">
    PathUtilities.TestAccessor :heavy_check_mark:
  </strong>
</summary>
<br>

- The `PathUtilities.TestAccessor` contains 1 members.
- 5 total lines of source code.
- Approximately 1 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [752](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/PathUtilities.cs#L752 "string? TestAccessor.GetDirectoryName(string path, bool isUnixLike)") | 98 | 1 :heavy_check_mark: | 0 | 1 | 2 / 1 |

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

</details>

</details>

<details>
<summary>
  <strong id="dotnet-codeanalysis-csharp">
    DotNet.CodeAnalysis.CSharp :heavy_check_mark:
  </strong>
</summary>
<br>

The `DotNet.CodeAnalysis.CSharp` namespace contains 1 named types.

- 1 named types.
- 11 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="csharpdefaults">
    CSharpDefaults :heavy_check_mark:
  </strong>
</summary>
<br>

- The `CSharpDefaults` contains 2 members.
- 8 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [11](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/CSharp/CSharpDefaults.cs#L11 "CSharpCommandLineParser CSharpDefaults.CommandLineParser") | 100 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |
| Property | [8](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/CSharp/CSharpDefaults.cs#L8 "CSharpCompilationOptions CSharpDefaults.CompilationOptions") | 100 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |

<a href="#dotnet-codeanalysis-csharp">:top: back to DotNet.CodeAnalysis.CSharp</a>

</details>

</details>

<details>
<summary>
  <strong id="dotnet-codeanalysis-visualbasic">
    DotNet.CodeAnalysis.VisualBasic :heavy_check_mark:
  </strong>
</summary>
<br>

The `DotNet.CodeAnalysis.VisualBasic` namespace contains 1 named types.

- 1 named types.
- 11 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="visualbasicdefaults">
    VisualBasicDefaults :heavy_check_mark:
  </strong>
</summary>
<br>

- The `VisualBasicDefaults` contains 2 members.
- 8 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [11](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/VisualBasic/VisualBasicDefaults.cs#L11 "VisualBasicCommandLineParser VisualBasicDefaults.CommandLineParser") | 100 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |
| Property | [8](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.CodeAnalysis/VisualBasic/VisualBasicDefaults.cs#L8 "VisualBasicCompilationOptions VisualBasicDefaults.CompilationOptions") | 100 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |

<a href="#dotnet-codeanalysis-visualbasic">:top: back to DotNet.CodeAnalysis.VisualBasic</a>

</details>

</details>

<a href="#dotnet-codeanalysis">:top: back to DotNet.CodeAnalysis</a>

<div id='dotnet-githubaction'></div>

## DotNet.GitHubAction :heavy_check_mark:

The *DotNet.GitHubAction.csproj* project file contains:

- 4 namespaces.
- 6 named types.
- 527 total lines of source code.
- Approximately 229 lines of executable code.
- The highest cyclomatic complexity is 7 :heavy_check_mark:.

<details>
<summary>
  <strong id="global+namespace">
    &lt;global namespace&gt; :heavy_check_mark:
  </strong>
</summary>
<br>

The `<global namespace>` namespace contains 1 named types.

- 1 named types.
- 112 total lines of source code.
- Approximately 92 lines of executable code.
- The highest cyclomatic complexity is 7 :heavy_check_mark:.

<details>
<summary>
  <strong id="program$">
    &lt;Program&gt;$ :heavy_check_mark:
  </strong>
</summary>
<br>

- The `<Program>$` contains 1 members.
- 112 total lines of source code.
- Approximately 92 lines of executable code.
- The highest cyclomatic complexity is 7 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [1](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Program.cs#L1 "<top-level-statements-entry-point>") | 43 | 7 :heavy_check_mark: | 0 | 19 | 112 / 46 |

<a href="#global+namespace">:top: back to &lt;global namespace&gt;</a>

</details>

</details>

<details>
<summary>
  <strong id="dotnet-githubaction-analyzers">
    DotNet.GitHubAction.Analyzers :heavy_check_mark:
  </strong>
</summary>
<br>

The `DotNet.GitHubAction.Analyzers` namespace contains 1 named types.

- 1 named types.
- 44 total lines of source code.
- Approximately 13 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

<details>
<summary>
  <strong id="projectmetricdataanalyzer">
    ProjectMetricDataAnalyzer :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ProjectMetricDataAnalyzer` contains 3 members.
- 41 total lines of source code.
- Approximately 13 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [13](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Analyzers/ProjectMetricDataAnalyzer.cs#L13 "ILogger<ProjectMetricDataAnalyzer> ProjectMetricDataAnalyzer._logger") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Method | [15](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Analyzers/ProjectMetricDataAnalyzer.cs#L15 "ProjectMetricDataAnalyzer.ProjectMetricDataAnalyzer(ILogger<ProjectMetricDataAnalyzer> logger)") | 96 | 1 :heavy_check_mark: | 0 | 1 | 1 / 1 |
| Method | [17](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Analyzers/ProjectMetricDataAnalyzer.cs#L17 "Task<ImmutableArray<(string, CodeAnalysisMetricData)>> ProjectMetricDataAnalyzer.AnalyzeAsync(ProjectWorkspace workspace, string path, CancellationToken cancellation)") | 61 | 3 :heavy_check_mark: | 0 | 10 | 34 / 12 |

<a href="#dotnet-githubaction-analyzers">:top: back to DotNet.GitHubAction.Analyzers</a>

</details>

</details>

<details>
<summary>
  <strong id="dotnet-githubaction-extensions">
    DotNet.GitHubAction.Extensions :heavy_check_mark:
  </strong>
</summary>
<br>

The `DotNet.GitHubAction.Extensions` namespace contains 3 named types.

- 3 named types.
- 313 total lines of source code.
- Approximately 98 lines of executable code.
- The highest cyclomatic complexity is 5 :heavy_check_mark:.

<details>
<summary>
  <strong id="codeanalysismetricdataextensions">
    CodeAnalysisMetricDataExtensions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `CodeAnalysisMetricDataExtensions` contains 6 members.
- 42 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [27](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeAnalysisMetricDataExtensions.cs#L27 "int CodeAnalysisMetricDataExtensions.CountKind(CodeAnalysisMetricData metric, SymbolKind kind)") | 82 | 1 :heavy_check_mark: | 0 | 4 | 4 / 3 |
| Method | [24](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeAnalysisMetricDataExtensions.cs#L24 "int CodeAnalysisMetricDataExtensions.CountNamedTypes(CodeAnalysisMetricData metric)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |
| Method | [21](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeAnalysisMetricDataExtensions.cs#L21 "int CodeAnalysisMetricDataExtensions.CountNamespaces(CodeAnalysisMetricData metric)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |
| Method | [32](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeAnalysisMetricDataExtensions.cs#L32 "(int Complexity, string Emoji) CodeAnalysisMetricDataExtensions.FindHighestCyclomaticComplexity(CodeAnalysisMetricData metric)") | 73 | 1 :heavy_check_mark: | 0 | 4 | 12 / 6 |
| Method | [45](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeAnalysisMetricDataExtensions.cs#L45 "IEnumerable<TSource> CodeAnalysisMetricDataExtensions.Flatten<TSource>(IEnumerable<TSource> parent, Func<TSource, IEnumerable<TSource>> childSelector)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 5 / 2 |
| Method | [11](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeAnalysisMetricDataExtensions.cs#L11 "string CodeAnalysisMetricDataExtensions.ToCyclomaticComplexityEmoji(CodeAnalysisMetricData metric)") | 88 | 1 :heavy_check_mark: | 0 | 2 | 9 / 1 |

<a href="#dotnet-githubaction-extensions">:top: back to DotNet.GitHubAction.Extensions</a>

</details>

<details>
<summary>
  <strong id="codemetricsreportextensions">
    CodeMetricsReportExtensions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `CodeMetricsReportExtensions` contains 13 members.
- 255 total lines of source code.
- Approximately 83 lines of executable code.
- The highest cyclomatic complexity is 5 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [141](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L141 "void CodeMetricsReportExtensions.AppendMaintainedByBotMessage(MarkdownDocument document)") | 98 | 1 :heavy_check_mark: | 0 | 3 | 3 / 1 |
| Method | [116](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L116 "void CodeMetricsReportExtensions.AppendMetricDefinitions(MarkdownDocument document)") | 66 | 2 :heavy_check_mark: | 0 | 5 | 24 / 7 |
| Method | [230](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L230 "IMarkdownDocument CodeMetricsReportExtensions.CloseCollapsibleSection(IMarkdownDocument document)") | 98 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |
| Method | [233](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L233 "IMarkdownDocument CodeMetricsReportExtensions.DisableMarkdownLinterAndCaptureConfig(IMarkdownDocument document)") | 98 | 1 :heavy_check_mark: | 0 | 2 | 4 / 1 |
| Method | [145](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L145 "string CodeMetricsReportExtensions.DisplayName(ISymbol symbol)") | 69 | 2 :heavy_check_mark: | 0 | 3 | 15 / 7 |
| Method | [211](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L211 "IMarkdownDocument CodeMetricsReportExtensions.OpenCollapsibleSection(IMarkdownDocument document, string elementId, string symbolName, string highestComplexity)") | 91 | 1 :heavy_check_mark: | 0 | 2 | 9 / 1 |
| Method | [221](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L221 "string CodeMetricsReportExtensions.PrepareElementId(string value)") | 88 | 1 :heavy_check_mark: | 0 | 1 | 8 / 1 |
| Method | [238](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L238 "IMarkdownDocument CodeMetricsReportExtensions.RestoreMarkdownLinter(IMarkdownDocument document)") | 98 | 1 :heavy_check_mark: | 0 | 2 | 3 / 1 |
| Method | [161](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L161 "string CodeMetricsReportExtensions.ToDisplayName(CodeAnalysisMetricData metric)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 14 / 1 |
| Method | [200](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L200 "(string elementId, string displayName, string anchorLink, (int highestComplexity, string emoji)) CodeMetricsReportExtensions.ToIdAndAnchorPair(CodeAnalysisMetricData metric)") | 71 | 2 :heavy_check_mark: | 0 | 5 | 10 / 5 |
| Method | [242](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L242 "string CodeMetricsReportExtensions.ToLineNumberUrl(ISymbol symbol, string symbolDisplayName, ActionInputs actionInputs)") | 61 | 4 :heavy_check_mark: | 0 | 3 | 26 / 10 |
| Method | [16](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L16 "string CodeMetricsReportExtensions.ToMarkDownBody(Dictionary<string, CodeAnalysisMetricData> metricData, ActionInputs actionInputs)") | 44 | 5 :heavy_check_mark: | 0 | 14 | 99 / 39 |
| Method | [176](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/CodeMetricsReportExtensions.cs#L176 "MarkdownTableRow CodeMetricsReportExtensions.ToTableRowFrom(CodeAnalysisMetricData metric, Func<CodeAnalysisMetricData, string> toDisplayName, ActionInputs actionInputs)") | 65 | 1 :heavy_check_mark: | 0 | 6 | 23 / 8 |

<a href="#dotnet-githubaction-extensions">:top: back to DotNet.GitHubAction.Extensions</a>

</details>

<details>
<summary>
  <strong id="servicecollectionextensions">
    ServiceCollectionExtensions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ServiceCollectionExtensions` contains 1 members.
- 7 total lines of source code.
- Approximately 1 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [9](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/Extensions/ServiceCollectionExtensions.cs#L9 "IServiceCollection ServiceCollectionExtensions.AddGitHubActionServices(IServiceCollection services)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 4 / 1 |

<a href="#dotnet-githubaction-extensions">:top: back to DotNet.GitHubAction.Extensions</a>

</details>

</details>

<details>
<summary>
  <strong id="dotnet-githubaction">
    DotNet.GitHubAction :heavy_check_mark:
  </strong>
</summary>
<br>

The `DotNet.GitHubAction` namespace contains 1 named types.

- 1 named types.
- 58 total lines of source code.
- Approximately 26 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

<details>
<summary>
  <strong id="actioninputs">
    ActionInputs :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ActionInputs` contains 9 members.
- 55 total lines of source code.
- Approximately 26 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [9](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L9 "string ActionInputs._branchName") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Field | [8](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L8 "string ActionInputs._repositoryName") | 93 | 0 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [11](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L11 "ActionInputs.ActionInputs()") | 82 | 2 :heavy_check_mark: | 0 | 1 | 8 / 3 |
| Property | [37](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L37 "string ActionInputs.Branch") | 93 | 2 :heavy_check_mark: | 0 | 2 | 8 / 5 |
| Property | [46](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L46 "string ActionInputs.Directory") | 100 | 2 :heavy_check_mark: | 0 | 2 | 4 / 3 |
| Property | [28](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L28 "string ActionInputs.Name") | 93 | 2 :heavy_check_mark: | 0 | 2 | 8 / 5 |
| Property | [23](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L23 "string ActionInputs.Owner") | 100 | 2 :heavy_check_mark: | 0 | 2 | 4 / 3 |
| Method | [53](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L53 "void ActionInputs.ParseAndAssign(string? value, Action<string> assign)") | 82 | 3 :heavy_check_mark: | 0 | 4 | 7 / 2 |
| Property | [51](https://github.com/dotnet/samples/blob/main/github-actions/DotNet.GitHubAction/DotNet.GitHubAction/ActionInputs.cs#L51 "string ActionInputs.WorkspaceDirectory") | 100 | 2 :heavy_check_mark: | 0 | 2 | 4 / 3 |

<a href="#dotnet-githubaction">:top: back to DotNet.GitHubAction</a>

</details>

</details>

<a href="#dotnet-githubaction">:top: back to DotNet.GitHubAction</a>

## Metric definitions

  - **Maintainability index**: Measures ease of code maintenance. Higher values are better.
  - **Cyclomatic complexity**: Measures the number of branches. Lower values are better.
  - **Depth of inheritance**: Measures length of object inheritance hierarchy. Lower values are better.
  - **Class coupling**: Measures the number of classes that are referenced. Lower values are better.
  - **Lines of source code**: Exact number of lines of source code. Lower values are better.
  - **Lines of executable code**: Approximates the lines of executable code. Lower values are better.

*This file is maintained by a bot.*

<!-- markdownlint-restore -->
