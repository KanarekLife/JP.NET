using Lab02.Cli.Models;

namespace Lab02.Cli.Utilities;

public static class DirectoryAnalysisHelper
{
    public static List<FileSystemItem> AnalyzeDirectory(AnalysisOptions options)
    {
        if (!DirectoryValidator.ValidateDirectory(options.DirectoryPath))
        {
            throw new DirectoryNotFoundException($"Directory '{options.DirectoryPath}' not found or inaccessible.");
        }

        var items = new List<FileSystemItem>();
        var directoryPath = Path.GetFullPath(options.DirectoryPath);
        
        var processedItems = 0;
        
        ScanDirectoryRecursive(directoryPath, 0, options, items, ref processedItems);
        
        return items;
    }
    
    private static void ScanDirectoryRecursive(
        string path, 
        int currentDepth, 
        AnalysisOptions options, 
        List<FileSystemItem> items,
        ref int processedItems)
    {
        if (currentDepth > options.MaxDepth)
        {
            return;
        }

        if (!Directory.Exists(path))
        {
            return;
        }

        var files = DirectoryValidator.GetDirectoryFiles(path);
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            if (!fileInfo.Exists) continue;

            items.Add(new FileSystemItem
            {
                Path = fileInfo.FullName,
                Size = fileInfo.Length,
                Extension = fileInfo.Extension,
                Attributes = fileInfo.Attributes,
                Type = FileSystemItemType.File,
                Level = currentDepth
            });

            processedItems++;
        }

        var directories = DirectoryValidator.GetDirectoryDirectories(path);
        foreach (var dir in directories)
        {
            var dirInfo = new DirectoryInfo(dir);
            if (!dirInfo.Exists) continue;

            items.Add(new FileSystemItem
            {
                Path = dirInfo.FullName,
                Size = 0,
                Extension = string.Empty,
                Attributes = dirInfo.Attributes,
                Type = FileSystemItemType.Directory,
                Level = currentDepth
            });

            processedItems++;

            if (currentDepth < options.MaxDepth)
            {
                ScanDirectoryRecursive(dir, currentDepth + 1, options, items, ref processedItems);
            }
        }
    }
}