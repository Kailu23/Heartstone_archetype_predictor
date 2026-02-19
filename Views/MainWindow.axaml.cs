using Avalonia.Controls;

using Hearthstone_Archetype_Predictor.ViewModels;

namespace Hearthstone_Archetype_Predictor.Views;

public partial class MainWindow : Window {
	public MainWindow() {
		InitializeComponent();
	}

	private async void DeckStringInput_TextChanged(object? sender, TextChangedEventArgs e) {
		string? deckString = DeckStringInput.Text;
		if (deckString is null) return;
		if (DataContext is MainWindowViewModel viewModel) {
			if (deckString.Length >= 60) {
				await viewModel.NewDeckString(deckString);
			}
		}
	}
}
