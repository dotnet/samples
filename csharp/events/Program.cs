var fileLister = new FileSearcher();
int filesFound = 0;

EventHandler<FileFoundArgs> onFileFound = (sender, eventArgs) =>
{
    Console.WriteLine(eventArgs.FoundFile);
    filesFound++;
};

fileLister.FileFound += onFileFound;

fileLister.DirectoryChanged += (sender, eventArgs) =>
{
    Console.Write($"Entering '{eventArgs.CurrentSearchDirectory}'.");
    Console.WriteLine($" {eventArgs.CompletedDirs} of {eventArgs.TotalDirs} completed...");
};

fileLister.Search(".", "*.dll", true);

fileLister.FileFound -= onFileFound;

public class FileFoundArgs : EventArgs
{
    public string FoundFile { get; }
    public bool CancelRequested { get; set; }

    public FileFoundArgs(string fileName) => FoundFile = fileName;
}

internal class SearchDirectoryArgs : EventArgs
{
    internal string CurrentSearchDirectory { get; }
    internal int TotalDirs { get; }
    internal int CompletedDirs { get; }

    internal SearchDirectoryArgs(string dir, int totalDirs, int completedDirs)
    {
        CurrentSearchDirectory = dir;
        TotalDirs = totalDirs;
        CompletedDirs = completedDirs;
    }
}

public class FileSearcher
{
    public event EventHandler<FileFoundArgs>? FileFound;
    internal event EventHandler<SearchDirectoryArgs>? DirectoryChanged
    {
        add => directoryChanged += value; 
        remove => directoryChanged -= value;
    }
    private EventHandler<SearchDirectoryArgs>? directoryChanged;

    public void Search(string directory, string searchPattern, bool searchSubDirs = false)
    {
        if (searchSubDirs)
        {
            var allDirectories = Directory.GetDirectories(directory, "*.*", SearchOption.AllDirectories);
            var completedDirs = 0;
            var totalDirs = allDirectories.Length + 1;
            foreach (var dir in allDirectories)
            {
                directoryChanged?.Invoke(this, new (dir, totalDirs, completedDirs++));
                // Search 'dir' and its subdirectories for files that match the search pattern:
                SearchDirectory(dir, searchPattern);
            }
            // Include the Current Directory:
            directoryChanged?.Invoke(this, new (directory, totalDirs, completedDirs++));
            SearchDirectory(directory, searchPattern);
        }
        else
        {
            SearchDirectory(directory, searchPattern);
        }
    }

    private void SearchDirectory(string directory, string searchPattern)
    {
        foreach (var file in Directory.EnumerateFiles(directory, searchPattern))
        {
            var args = new FileFoundArgs(file);
            FileFound?.Invoke(this, args);
            if (args.CancelRequested)
                break;
        }
    }
}
