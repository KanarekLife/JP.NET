namespace Lab02.Cli.Models;

public class AnalysisOptions
{
    public string DirectoryPath { get; set; } = string.Empty;
    public int MaxDepth { get; set; } = 3;
    public string OutputPath { get; set; } = string.Empty;
}