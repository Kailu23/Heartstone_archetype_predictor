using Avalonia.Controls;
using Hearthstone_Archetype_Predictor.ViewModels;
using System.Threading.Tasks;

namespace Hearthstone_Archetype_Predictor.Views;

public partial class MainWindow : Window {
	public MainWindow() {
		InitializeComponent();
	}

	private async void OnListBoxLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.OnListBoxLoaded();
        }
    }
}
