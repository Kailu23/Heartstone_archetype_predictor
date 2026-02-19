using Avalonia.Media.Imaging;
using Hearthstone_Archetype_Predictor.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Hearthstone_Archetype_Predictor.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged {

	private HearthstoneSerializer _hearthstoneSerializer;

	public ObservableCollection<Bitmap> Images { get; private set; }

	private ObservableCollection<string> _cardNames;
	public ObservableCollection<string> CardNames {
		get {
			return _cardNames;
		}
	}
	private ObservableCollection<int> _cardCopies;
	public ObservableCollection<int> CardCopies {
		get { return _cardCopies; }
	}

	private ObservableCollection<string> _cardNamesAndCopies;
	public ObservableCollection<string> CardNamesAndCopies {
		get { return _cardNamesAndCopies; }
	}

	public MainWindowViewModel() {
		this._hearthstoneSerializer = new HearthstoneSerializer();
		this.Images = new ObservableCollection<Bitmap>();
		this._cardNames = new ObservableCollection<string>();
		this._cardCopies = new ObservableCollection<int>();
		this._cardNamesAndCopies = new ObservableCollection<string>();
	}

	public async Task GetImagesAsync() {
		List<Bitmap> images = await _hearthstoneSerializer.GetImagesAsync();

		Images.Clear();
		foreach (Bitmap image in images) {
			Images.Add(image);
		}
		FormatCardNamesAndCopies();
	}

	private void FormatCardNamesAndCopies() {
		string fullCard;
		_cardNamesAndCopies.Clear();
		foreach (var card in _hearthstoneSerializer.Cards) {
			fullCard = $"{card.Value}\t{card.Key.Name}";
			_cardNamesAndCopies.Add(fullCard);
		}
	}
	public async Task NewDeckString(string deckString) {
		_hearthstoneSerializer.DeckString = deckString;
		await GetImagesAsync();
	}
}
