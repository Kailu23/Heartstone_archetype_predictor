using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Heartstone_Archetype_Predictor.Models;
using Heartstone_Archetype_Predictor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Heartstone_Archetype_Predictor.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged {

    private readonly HearthstoneSerializer _hearthstoneSerializer;

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

    public virtual async Task OnListBoxLoaded() {
        await GetImagesAsync();
    }
    public MainWindowViewModel() {
        this._hearthstoneSerializer = new HearthstoneSerializer("AAEDAaa4AwTTlQSvlgT6oASPowQN25UE3JUEppYEsJYEtpYEvZYE1JYE3ZYE6aEE8KEE8aEE86EE1KIEAA==");
        this.Images = new ObservableCollection<Bitmap>();
        this._cardNames = new ObservableCollection<string>();
        this._cardCopies = new ObservableCollection<int>();
    }

    public async Task GetImagesAsync() {
        List<Bitmap> images = await _hearthstoneSerializer.GetImagesAsync();

        foreach (Bitmap image in images) {
            Images.Add(image);
        }
        GetCardNames();
    }
    private void GetCardCopies() {
        foreach (var card in _hearthstoneSerializer.Cards)
        {
            _cardCopies.Add(card.Value);
        }
    }
    private void GetCardNames() {
        foreach (var card in _hearthstoneSerializer.Cards) {
            _cardNames.Add(card.Key.Name);
        }
        GetCardCopies();
    }
}
