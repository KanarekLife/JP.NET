namespace Lab02.Cli.Utilities;

public static class DirectoryValidator
{
    public static bool ValidateDirectory(string directoryPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return false;

            directoryPath = Path.GetFullPath(directoryPath);
            
            if (!Directory.Exists(directoryPath))
                return false;
            
            Directory.GetFiles(directoryPath);
            Directory.GetDirectories(directoryPath);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static string[] GetDirectoryFiles(string path) 
        => Directory.GetFiles(path).Where(f => !FileHelper.IsHidden(f)).ToArray();

    public static string[] GetDirectoryDirectories(string path)
        => Directory.GetDirectories(path).Where(d => !FileHelper.IsHidden(d)).ToArray();
}