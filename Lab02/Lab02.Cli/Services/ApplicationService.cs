using Lab02.Cli.Models;
using Lab02.Cli.Services;
using Lab02.Cli.Utilities;

namespace Lab02.Cli.Services;

public static class ApplicationService
{
    public static AnalysisResult PerformAnalysis(AnalysisOptions options)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(options.DirectoryPath))
            {
                return new AnalysisResult(false, ErrorMessage: "Directory path cannot be empty.");
            }
            
            if (options.MaxDepth < 0)
            {
                return new AnalysisResult(false, ErrorMessage: "Depth must be a non-negative number.");
            }
            
            if (string.IsNullOrWhiteSpace(options.OutputPath))
            {
                return new AnalysisResult(false, ErrorMessage: "Output path cannot be empty.");
            }
            
            options.DirectoryPath = Path.GetFullPath(options.DirectoryPath);
            options.OutputPath = Path.GetFullPath(options.OutputPath);
            
            if (!DirectoryValidator.ValidateDirectory(options.DirectoryPath))
            {
                return new AnalysisResult(false, ErrorMessage: $"Directory '{options.DirectoryPath}' does not exist or is not accessible.");
            }
            
            var items = DirectoryAnalysisHelper.AnalyzeDirectory(options);
            
            ExcelReportService.GenerateReport(items, options.OutputPath);
            
            var files = items.Where(i => i.Type == FileSystemItemType.File).ToList();
            var directories = items.Where(i => i.Type == FileSystemItemType.Directory).ToList();
            
            var extensionCount = files
                .GroupBy(f => string.IsNullOrEmpty(f.Extension) ? "No extension" : f.Extension.ToLower())
                .Count();
            
            return new AnalysisResult(
                Success: true,
                OutputPath: options.OutputPath,
                DirectoryCount: directories.Count,
                FileCount: files.Count,
                ExtensionCount: extensionCount
            );
        }
        catch (Exception ex)
        {
            return new AnalysisResult(false, ErrorMessage: ex.Message);
        }
    }
}