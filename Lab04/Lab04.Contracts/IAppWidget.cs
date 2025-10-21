using System.Windows.Controls;

namespace Lab04.Contracts;

public static class AppWidgetMetadata
{
    public const string Name = "Name";
}

public interface IAppWidget
{
    string Name { get; }
    UserControl Control { get; }
}
