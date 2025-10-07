using Lab02.Cli.UI;

namespace Lab02.Cli;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--console")
        {
            ConsoleManager.Run(args.Skip(1).ToArray());
            return;
        }

        try
        {
            GuiManager.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GUI mode failed: {ex.Message}");
            Console.WriteLine("Falling back to console mode. Use --console <directory> <depth> for direct console usage.");
            Console.WriteLine();
            
            ConsoleManager.Run([Environment.CurrentDirectory, "3"]);
        }
    }
}