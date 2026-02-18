using Avalonia.Controls;
using Heartstone_Archetype_Predictor.ViewModels;
using System.Threading.Tasks;

namespace Heartstone_Archetype_Predictor.Views;

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
