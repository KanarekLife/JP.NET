namespace Lab02.Cli.Utilities;

public static class FileHelper
{
    public static string FormatFileSize(long bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
        var counter = 0;
        decimal number = bytes;
        
        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }
        
        return $"{number:N1} {suffixes[counter]}";
    }
    
    public static string GetFileAttributesString(FileAttributes attributes)
    {
        var attrList = new List<string>();
        
        if (attributes.HasFlag(FileAttributes.ReadOnly)) attrList.Add("ReadOnly");
        if (attributes.HasFlag(FileAttributes.Hidden)) attrList.Add("Hidden");
        if (attributes.HasFlag(FileAttributes.System)) attrList.Add("System");
        if (attributes.HasFlag(FileAttributes.Archive)) attrList.Add("Archive");
        if (attributes.HasFlag(FileAttributes.Compressed)) attrList.Add("Compressed");
        if (attributes.HasFlag(FileAttributes.Encrypted)) attrList.Add("Encrypted");
        
        return attrList.Count > 0 ? string.Join(", ", attrList) : "Normal";
    }
    
    public static bool IsHidden(string path)
    {
        try
        {
            var attributes = File.GetAttributes(path);
            return attributes.HasFlag(FileAttributes.Hidden);
        }
        catch
        {
            return false;
        }
    }
}