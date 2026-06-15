using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using HearthDb.Deckstrings;
using Hearthstone_Archetype_Predictor.ViewModels;

namespace Hearthstone_Archetype_Predictor.Views;

public partial class MainWindow : Window {
    /// <summary>
    /// Holds the most recently pasted deck strings
    /// </summary>
    private string? _lastPastedDeckString;

    public MainWindow() {
        InitializeComponent();
        AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
        this.Opened += OnOpened;
    }

    private async void OnPreviewKeyDown(object? sender, KeyEventArgs e) {
        bool isPaste = e.Key == Key.V && e.KeyModifiers.HasFlag(KeyModifiers.Control);
        if (isPaste is false)
            return;

        await HandleClipboardPasteAsync();
    }

    private async Task HandleClipboardPasteAsync() {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
            return;

        string? pastedText = await clipboard.TryGetTextAsync();
        if (string.IsNullOrWhiteSpace(pastedText))
            return;

        _lastPastedDeckString = pastedText;

        if (DataContext is not MainWindowViewModel viewModel)
            return;

        bool isValid = await viewModel.NewDeckString(_lastPastedDeckString);

        if (isValid is false) {
            await ShowAlertAsync(
                "Invalid deck string",
                "The text on your clipboard doesn't look like a valid Hearthstone deck string."
            );
        }
    }

    private async Task ShowAlertAsync(string title, string message) {
        var alert = new AlertWindow(title, message);
        await alert.ShowDialog(this);
    }

    private void OnOpened(object? sender, System.EventArgs e) {
        var screen = Screens.Primary;
        if (screen is not null) {
            double density = screen.Scaling;
            double screenWidth = screen.WorkingArea.Width / density;
            double screenHeight = screen.WorkingArea.Height / density;

            const double WidthFraction = 1;
            const double HeightFraction = 0.75;
            Width = screenWidth * WidthFraction;
            Height = screenHeight * HeightFraction;

            // Re-centre after resize
            var pos = screen.WorkingArea;
            Position = new PixelPoint(
                (int) (pos.X + (pos.Width - Width * density) / 2),
                (int) (pos.Y + (pos.Height - Height * density) / 2)
            );
        }
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e) {
        base.OnSizeChanged(e);
        if (DataContext is MainWindowViewModel vm) {
            vm.WindowWidth = e.NewSize.Width;
            vm.WindowHeight = e.NewSize.Height;
        }
    }
}
