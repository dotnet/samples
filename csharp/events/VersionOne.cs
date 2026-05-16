namespace Events;

public class FileFoundArgs : EventArgs
{
    public string FoundFile { get; }

    public FileFoundArgs(string fileName) => FoundFile = fileName;
}

public class FileSearcher
{
    public event EventHandler<FileFoundArgs>? FileFound;

    public void Search(string directory, string searchPattern)
    {
        var enumeratedFiles = Directory.EnumerateFiles(directory, searchPattern);

        foreach (var file in enumeratedFiles)
        {
            FileFound?.Invoke(this, new FileFoundArgs(file));
        }
    }
}