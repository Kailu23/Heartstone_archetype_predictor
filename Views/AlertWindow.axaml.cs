using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Hearthstone_Archetype_Predictor.Views;

public partial class AlertWindow : Window {
    public AlertWindow() {
        InitializeComponent();
    }

    public AlertWindow(string title, string message)
        : this() {
        Title = title;
        MessageText.Text = message;
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e) {
        Close();
    }
}
