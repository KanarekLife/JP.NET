namespace Lab02.Cli.Models;

public class FileSystemItem
{
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Extension { get; set; } = string.Empty;
    public FileAttributes Attributes { get; set; }
    public FileSystemItemType Type { get; set; }
    public int Level { get; set; }
}

public enum FileSystemItemType
{
    File,
    Directory
}
