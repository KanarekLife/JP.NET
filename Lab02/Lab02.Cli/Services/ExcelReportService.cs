using Lab02.Cli.Models;
using Lab02.Cli.Utilities;
using OfficeOpenXml;

namespace Lab02.Cli.Services;

public static class ExcelReportService
{
    public static void GenerateReport(List<FileSystemItem> items, string outputPath)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Lab02 User");
        using var package = new ExcelPackage();
        CreateMainWorksheet(package, items);
        CreateStatisticsWorksheet(package, items);
        SaveExcelFile(package, outputPath);
    }
    
    private static void CreateMainWorksheet(ExcelPackage package, List<FileSystemItem> items)
    {
        var worksheet = package.Workbook.Worksheets.Add("Directory Structure");
        
        worksheet.Cells[1, 1].Value = "Path";
        worksheet.Cells[1, 2].Value = "Type";
        worksheet.Cells[1, 3].Value = "Level";
        worksheet.Cells[1, 4].Value = "Extension";
        worksheet.Cells[1, 5].Value = "Size";
        worksheet.Cells[1, 6].Value = "Attributes";
        
        worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
        worksheet.Cells[1, 1, 1, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[1, 1, 1, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        var currentRow = 2;
        foreach (var item in items)
        {
            worksheet.Cells[currentRow, 1].Value = item.Path;
            worksheet.Cells[currentRow, 2].Value = item.Type == FileSystemItemType.File ? "File" : "Directory";
            worksheet.Cells[currentRow, 3].Value = item.Level;
            worksheet.Cells[currentRow, 4].Value = item.Type == FileSystemItemType.File ? item.Extension : "-";
            worksheet.Cells[currentRow, 5].Value = item.Type == FileSystemItemType.File ? FileHelper.FormatFileSize(item.Size) : "-";
            worksheet.Cells[currentRow, 6].Value = FileHelper.GetFileAttributesString(item.Attributes);
            
            worksheet.Cells[currentRow, 1].Style.Indent = item.Level;
            if (item.Type == FileSystemItemType.Directory)
            {
                worksheet.Cells[currentRow, 1, currentRow, 6].Style.Font.Bold = true;
            }
            
            if (item.Level > 0)
            {
                worksheet.Row(currentRow).OutlineLevel = Math.Min(item.Level, 7);
            }
            
            currentRow++;
        }
        
        worksheet.Cells.AutoFitColumns();
        
        worksheet.OutLineApplyStyle = true;
        worksheet.OutLineSummaryBelow = false;
        worksheet.OutLineSummaryRight = false;
    }
    
    private static void CreateStatisticsWorksheet(ExcelPackage package, List<FileSystemItem> items)
    {
        var files = items.Where(i => i.Type == FileSystemItemType.File).ToList();
        if (files.Count == 0)
        {
            return;
        }
        
        var statsWorksheet = package.Workbook.Worksheets.Add("Statistics");
        
        statsWorksheet.Cells[1, 1].Value = "Ranking";
        statsWorksheet.Cells[1, 2].Value = "File Path";
        statsWorksheet.Cells[1, 3].Value = "Size";
        statsWorksheet.Cells[1, 4].Value = "Size (bytes)";
        statsWorksheet.Cells[1, 5].Value = "Extension";
        statsWorksheet.Cells[1, 6].Value = "Attributes";
        
        statsWorksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
        statsWorksheet.Cells[1, 1, 1, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        statsWorksheet.Cells[1, 1, 1, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        statsWorksheet.Cells[1, 1, 1, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        
        var topFiles = files
            .OrderByDescending(f => f.Size)
            .Take(10)
            .ToList();
        
        for (var i = 0; i < topFiles.Count; i++)
        {
            var file = topFiles[i];
            var row = i + 2;
            
            statsWorksheet.Cells[row, 1].Value = i + 1;
            statsWorksheet.Cells[row, 2].Value = file.Path;
            statsWorksheet.Cells[row, 3].Value = FileHelper.FormatFileSize(file.Size);
            statsWorksheet.Cells[row, 4].Value = file.Size;
            statsWorksheet.Cells[row, 5].Value = file.Extension;
            statsWorksheet.Cells[row, 6].Value = FileHelper.GetFileAttributesString(file.Attributes);
        }
        
        statsWorksheet.Cells.AutoFitColumns();
        
        if (statsWorksheet.Column(2).Width > 50)
        {
            statsWorksheet.Column(2).Width = 50;
            statsWorksheet.Cells[2, 2, topFiles.Count + 1, 2].Style.WrapText = true;
        }
        
        if (files.Count > 0)
        {
            CreateCharts(statsWorksheet, files);
        }
    }
    
    private static void CreateCharts(ExcelWorksheet statsWorksheet, List<FileSystemItem> files)
    {
        var extensionStats = files
            .GroupBy(f => string.IsNullOrEmpty(f.Extension) ? "No extension" : f.Extension.ToLower())
            .Select(g => new { Extension = g.Key, Count = g.Count(), TotalSize = g.Sum(f => f.Size) })
            .OrderByDescending(x => x.Count)
            .ToList();
        
        const int chartDataStartRow = 15;
        
        statsWorksheet.Cells[chartDataStartRow, 1].Value = "Extension";
        statsWorksheet.Cells[chartDataStartRow, 2].Value = "File Count";
        statsWorksheet.Cells[chartDataStartRow, 1, chartDataStartRow, 2].Style.Font.Bold = true;
        
        for (var i = 0; i < extensionStats.Count; i++)
        {
            var stat = extensionStats[i];
            statsWorksheet.Cells[chartDataStartRow + 1 + i, 1].Value = stat.Extension;
            statsWorksheet.Cells[chartDataStartRow + 1 + i, 2].Value = stat.Count;
        }
        
        const int sizeChartDataStartCol = 4;
        statsWorksheet.Cells[chartDataStartRow, sizeChartDataStartCol].Value = "Extension";
        statsWorksheet.Cells[chartDataStartRow, sizeChartDataStartCol + 1].Value = "Size (MB)";
        statsWorksheet.Cells[chartDataStartRow, sizeChartDataStartCol, chartDataStartRow, sizeChartDataStartCol + 1].Style.Font.Bold = true;
        
        for (var i = 0; i < extensionStats.Count; i++)
        {
            var stat = extensionStats[i];
            statsWorksheet.Cells[chartDataStartRow + 1 + i, sizeChartDataStartCol].Value = stat.Extension;
            statsWorksheet.Cells[chartDataStartRow + 1 + i, sizeChartDataStartCol + 1].Value = Math.Round(stat.TotalSize / (1024.0 * 1024.0), 2);
        }

        var countChart = statsWorksheet.Drawings.AddChart("CountChart", OfficeOpenXml.Drawing.Chart.eChartType.Pie);
        countChart.Title.Text = "Percentage share of files by extension (count)";
        countChart.SetPosition(2, 0, 7, 0);
        countChart.SetSize(400, 300);

        var countSeries = countChart.Series.Add(
            statsWorksheet.Cells[chartDataStartRow + 1, 2, chartDataStartRow + extensionStats.Count, 2],
            statsWorksheet.Cells[chartDataStartRow + 1, 1, chartDataStartRow + extensionStats.Count, 1]
        );
        countSeries.Header = "File Count";

        var sizeChart = statsWorksheet.Drawings.AddChart("SizeChart", OfficeOpenXml.Drawing.Chart.eChartType.Pie);
        sizeChart.Title.Text = "Percentage share of files by extension (size)";
        sizeChart.SetPosition(18, 0, 7, 0);
        sizeChart.SetSize(400, 300);

        var sizeSeries = sizeChart.Series.Add(
            statsWorksheet.Cells[chartDataStartRow + 1, sizeChartDataStartCol + 1, chartDataStartRow + extensionStats.Count, sizeChartDataStartCol + 1],
            statsWorksheet.Cells[chartDataStartRow + 1, sizeChartDataStartCol, chartDataStartRow + extensionStats.Count, sizeChartDataStartCol]
        );
        sizeSeries.Header = "Size (MB)";
    }
    
    private static void SaveExcelFile(ExcelPackage package, string outputPath)
    {
        try
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            if (File.Exists(outputPath))
            {
                using var testStream = File.OpenWrite(outputPath);
            }
            
            package.SaveAs(new FileInfo(outputPath));
            
            if (!File.Exists(outputPath))
            {
                throw new IOException("File was not created.");
            }
            
            var fileInfo = new FileInfo(outputPath);
            if (fileInfo.Length == 0)
            {
                throw new IOException("File was created but is empty.");
            }
        }
        catch (Exception ex)
        {
            throw new IOException($"Error saving file: {ex.Message}", ex);
        }
    }
}