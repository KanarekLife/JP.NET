using System.Windows.Controls;
using Lab04.Contracts;

namespace Lab04.TextWidget;

public partial class TextWidget : UserControl
{
    public TextWidget()
    {
        InitializeComponent();
    }

    public void ApplyEvent(DataUpdatedEventValue @event)
    {
        Dispatcher.Invoke(() =>
        {
            ReceivedTextBlock.Text = @event.Data;
            CharCountText.Text = @event.Data.Length.ToString();
            WordCountText.Text = @event.Data.Split([' ', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries).Length.ToString();
        });
    }
}