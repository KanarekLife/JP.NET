using Lab02.Cli.Models;
using Lab02.Cli.Services;
using Terminal.Gui;

namespace Lab02.Cli.UI;

public static class GuiManager
{
    public static void Run()
    {
        try
        {
            Application.Init();
            
            var top = Application.Top;
            
            var window = new Window("Program")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            
            top.Add(window);
            
            CreateControls(window);
            Application.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GUI Error: {ex.Message}");
            Console.WriteLine("Fallback to console mode. Use --console flag for console interface.");
        }
        finally
        {
            try
            {
                Application.Shutdown();
            }
            catch
            {
                // Ignore
            }
        }
    }
    
    private static void CreateControls(Window window)
    {
        var lblDirectory = new Label("Directory path:") { X = 2, Y = 2 };
        var txtDirectory = new TextField() { X = 2, Y = 3, Width = Dim.Fill() - 4 };
        txtDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        var btnBrowseDir = new Button("Browse...") { X = 2, Y = 4 };
        btnBrowseDir.Clicked += () => BrowseDirectory(txtDirectory);
        
        var lblDepth = new Label("Search depth:") { X = 2, Y = 6 };
        var txtDepth = new TextField("3") { X = 2, Y = 7, Width = 10 };
        
        var lblOutputPath = new Label("Excel file output path:") { X = 2, Y = 9 };
        var txtOutputPath = new TextField() { X = 2, Y = 10, Width = Dim.Fill() - 4 };
        txtOutputPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "directory_structure.xlsx");
        
        var btnBrowseOutput = new Button("Browse...") { X = 2, Y = 11 };
        btnBrowseOutput.Clicked += () => BrowseOutputFile(txtOutputPath);
        
        var btnStart = new Button("Start Analysis") { X = 2, Y = 20 };
        var btnExit = new Button("Exit") { X = Pos.Right(btnStart) + 2, Y = 20 };
        
        var lblStatus = new Label("Ready for analysis...") { X = 2, Y = 22, Width = Dim.Fill() - 4 };
        
        btnStart.Clicked += () => StartAnalysis(
            txtDirectory.Text.ToString()!,
            txtDepth.Text.ToString()!,
            txtOutputPath.Text.ToString()!,
            lblStatus
        );
        
        btnExit.Clicked += () => Application.RequestStop();
        
        window.Add(
            lblDirectory, txtDirectory, btnBrowseDir,
            lblDepth, txtDepth,
            lblOutputPath, txtOutputPath, btnBrowseOutput,
            btnStart, btnExit,
            lblStatus
        );
    }    
    
    private static void BrowseDirectory(TextField textField)
    {
        var dialog = new OpenDialog("Select Directory", "Select directory to analyze")
        {
            CanChooseDirectories = true,
            CanChooseFiles = false,
            AllowsMultipleSelection = false
        };
        
        Application.Run(dialog);
        
        if (!dialog.Canceled && dialog.FilePaths.Count > 0)
        {
            textField.Text = dialog.FilePaths[0];
        }
    }
    
    private static void BrowseOutputFile(TextField textField)
    {
        var dialog = new SaveDialog("Save As", "Select Excel file location")
        {
            AllowedFileTypes = [".xlsx"]
        };
        
        Application.Run(dialog);
        
        if (!dialog.Canceled && !string.IsNullOrEmpty(dialog.FileName?.ToString()))
        {
            var fileName = dialog.FileName.ToString();
            if (!fileName!.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".xlsx";
            }
            textField.Text = fileName;
        }
    }

    private static void StartAnalysis(string directoryPath, string depthText, string outputPath, Label statusLabel)
    {
        Task.Run(() => {
            try
            {
                if (!int.TryParse(depthText, out var depth) || depth < 0)
                {
                    Application.MainLoop.Invoke(() => {
                        MessageBox.ErrorQuery("Error", "Depth must be a non-negative number!", "OK");
                    });
                    return;
                }
                
                var options = new AnalysisOptions
                {
                    DirectoryPath = directoryPath,
                    MaxDepth = depth,
                    OutputPath = outputPath
                };
                
                Application.MainLoop.Invoke(() => {
                    statusLabel.Text = "Starting analysis...";
                });
                
                var result = ApplicationService.PerformAnalysis(options);
                
                Application.MainLoop.Invoke(() => {
                    if (result.Success)
                    {
                        statusLabel.Text = $"Analysis completed! File: {result.OutputPath}";
                        MessageBox.Query("Success!",
                            $"Analysis completed successfully!\n\n" +
                            $"File: {result.OutputPath}\n" +
                            $"Directories: {result.DirectoryCount}\n" +
                            $"Files: {result.FileCount}\n" +
                            $"Extensions: {result.ExtensionCount}", "OK");
                    }
                    else
                    {
                        statusLabel.Text = "Analysis failed";
                        MessageBox.ErrorQuery("Error", $"{result.ErrorMessage}", "OK");
                    }
                });
            }
            catch (Exception ex)
            {
                Application.MainLoop.Invoke(() => {
                    statusLabel.Text = "An error occurred";
                    MessageBox.ErrorQuery("Error", $"{ex.Message}", "OK");
                });
            }
        });
    }
}