namespace Lab02.Cli.Models;

public record AnalysisResult(
    bool Success, 
    string? OutputPath = null, 
    string? ErrorMessage = null, 
    int DirectoryCount = 0, 
    int FileCount = 0, 
    int ExtensionCount = 0
);