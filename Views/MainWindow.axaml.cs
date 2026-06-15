using System;
using System.Threading.Tasks;
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
    }

    private async void OnPreviewKeyDown(object? sender, KeyEventArgs e) {
        bool isPaste = e.Key == Key.V && e.KeyModifiers.HasFlag(KeyModifiers.Control);
        if (isPaste is false || DeckStringInput.IsFocused)
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

    private async void DeckStringInput_TextChanged(object? sender, TextChangedEventArgs e) {
        string? deckString = DeckStringInput.Text;
        if (deckString is null)
            return;
        if (DataContext is MainWindowViewModel viewModel) {
            if (deckString.Length >= 60) {
                bool isValid = await viewModel.NewDeckString(deckString);
                if (isValid is false) {
                    await ShowAlertAsync(
                        "Invalid deck string",
                        "The text on your clipboard doesn't look like a valid Hearthstone deck string."
                    );
                }
            }
        }
    }
}
