using Lab02.Cli.Models;
using Lab02.Cli.Services;
using Lab02.Cli.Utilities;

namespace Lab02.Cli.UI;

public static class ConsoleManager
{
    public static void Run(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: dotnet run --console <directory_path> <search_depth> [output_path]");
            return;
        }

        var directoryPath = args[0];
        if (!int.TryParse(args[1], out var maxDepth) || maxDepth < 0)
        {
            Console.WriteLine("Error: Search depth must be a non-negative number.");
            return;
        }

        var outputPath = args.Length > 2 ? args[2] : Path.Combine(Environment.CurrentDirectory, "directory_structure.xlsx");

        if (!DirectoryValidator.ValidateDirectory(directoryPath))
        {
            Console.WriteLine($"Error: Directory '{directoryPath}' does not exist or is not accessible.");
            return;
        }

        try
        {
            var options = new AnalysisOptions
            {
                DirectoryPath = directoryPath,
                MaxDepth = maxDepth,
                OutputPath = outputPath
            };

            var result = ApplicationService.PerformAnalysis(options);

            Console.WriteLine();

            Console.WriteLine(result.Success
                ? $"\nExcel document created: {result.OutputPath}"
                : $"{result.ErrorMessage}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}