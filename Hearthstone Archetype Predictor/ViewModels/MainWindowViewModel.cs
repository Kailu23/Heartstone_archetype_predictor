using Avalonia.Media.Imaging;
using Hearthstone_Archetype_Predictor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Hearthstone_Archetype_Predictor.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged {

    private HearthstoneSerializer _hearthstoneSerializer;
    private AzureMLClient _azureMLClient;

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

    private string _archetype = "...";
    public string Archetype
    {
        get { return _archetype; }
        set { _archetype = value; OnPropertyChanged(nameof(Archetype));}
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public double WindowHeight, WindowWidth;
    public double Scale => Math.Min(WindowWidth / 900.0, WindowHeight / 850.0);
    public double FontSizeTitle => 28 * Scale;
    public double FontSizeNormal => 16 * Scale;
    public double FontSizeSmall => 14 * Scale;
    public double ImageWidth => 315 * Scale;
    public double ImageHeight => 512 * Scale;

    public MainWindowViewModel() {
        this._hearthstoneSerializer = new HearthstoneSerializer();
        this._azureMLClient = new AzureMLClient();
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
        await PredictArchetypeAsync();
    }

    private async Task PredictArchetypeAsync()
    {
        try
        {
            Archetype = "Predicting...";
            var (deckClass, cardIds) = _hearthstoneSerializer.GetDeckClassAndCardIds();
            string archetype = await _azureMLClient.PredictArchetypeAsync(deckClass, cardIds);
            Archetype = archetype;
        }
        catch (Exception ex)
        {
            Archetype= "Unknown";
            Console.WriteLine($"Error predicting archetype: {ex.Message}");
        }
    }

    private void FormatCardNamesAndCopies() {
        string fullCard;
        _cardNamesAndCopies.Clear();
        foreach (var card in _hearthstoneSerializer.Cards) {
            fullCard = $"{card.Value}\t{card.Key.Name}";
            _cardNamesAndCopies.Add(fullCard);
        }
    }
    public async Task<bool> NewDeckstring(string input) {
        string deckString = HearthstoneSerializer.ExtractDeckString(input);

        if (_hearthstoneSerializer.IsValidDeckString(deckString) is false) return false;

        _hearthstoneSerializer.DeckString = deckString;
        await GetImagesAsync();
        return true;
    }
}
