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

    public virtual async Task OnListBoxLoaded() {
        await GetImagesAsync();
    }
    public MainWindowViewModel() {
        this._hearthstoneSerializer = new HearthstoneSerializer();
        this.Images = new ObservableCollection<Bitmap>();
        this._cardNames = new ObservableCollection<string>();
    }

    private void GetCardNames() {
        foreach (var card in _hearthstoneSerializer.Cards) {
            _cardNames.Add(card.Key.Name);
        }
    }
    public async Task GetImagesAsync() {
        List<Bitmap> images = await _hearthstoneSerializer.GetImagesAsync();

        foreach (Bitmap image in images)
        {
            Images.Add(image);
        }
        GetCardNames();
    }
}
