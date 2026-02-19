using System;
using System.Collections.Generic;
using HearthDb.Deckstrings;
using Tmds.DBus.Protocol;
using Avalonia.Controls;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Linq;
using Avalonia.Media.Imaging;

namespace Hearthstone_Archetype_Predictor.Models;

public class HearthstoneSerializer {

    private string _deckString;
    private Deck _deck;

    private Dictionary<HearthDb.Card, int> _cards;
    public Dictionary<HearthDb.Card, int> Cards {
        get {
            return this._cards;
        }
    }

    private List<Bitmap> _images;

    private Dictionary<HearthDb.Card, string> _imageUrls;

    private string baseUrl = "https://art.hearthstonejson.com/v1/render/latest/enUS/512x";

    public HearthstoneSerializer(string deckString = "AAECAQcCrwSRvAIOHLACkQP/A44FqAXUBaQG7gbnB+8HgrACiLACub8CAAA=") {
        this._deckString = deckString;
        this._deck = new Deck();
        this._cards = new Dictionary<HearthDb.Card, int>();
        this._images = new List<Bitmap>();
        this._imageUrls = new Dictionary<HearthDb.Card, string>();
    }

    public async Task<List<Bitmap>> GetImagesAsync() {
        await Deserialize(this._deckString);
        return _images;
    }
    public async Task<List<Bitmap>> ChangeDeck(string deckString) {
        this._deckString = deckString;
        return await GetImagesAsync();
    }

    private async Task Deserialize(string deckString) {
        List<Bitmap> downloadedBitmaps = new List<Bitmap>();
        this._deck = DeckSerializer.Deserialize(deckString);

        _cards = _deck.GetCards();

        string imageUrl;
        foreach (var card in _cards) {
            imageUrl = CreateImageUrl(card.Key.Id);
            _imageUrls.Add(card.Key, imageUrl);
        }

        downloadedBitmaps = await DownloadImagesAsync(_imageUrls);
        _images = downloadedBitmaps;
    }
    /// <summary>
    /// Creates an image url based on the card id.
    /// 
    /// https://art.hearthstonejson.com/v1/render/latest/{LOCALE}/{RESOLUTION}/{CARD_ID}.{EXT}
    /// 
    /// For LOCALE we chose english(US), resolution is 512x512 pixels. 
    /// 
    /// Only supports .png extension 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    private string CreateImageUrl(string id) {
        return $"{baseUrl}/{id}.png";
    }

    private async Task<List<Bitmap>> DownloadImagesAsync(Dictionary<HearthDb.Card, string> imageUrls) {

        try {
            var downloadTasks = imageUrls.Select(url => DownloadImageAsync(url.Value));

            var bitmaps = await Task.WhenAll(downloadTasks);

            return bitmaps.ToList();
        } catch (Exception ex) {
            Console.WriteLine($"Error downloading images: {ex.Message}");
            return new List<Bitmap>();
        }
    }

    private async Task<Bitmap> DownloadImageAsync(string imageUrl) {
        using var client = new HttpClient();
        using var response = await client.GetAsync(imageUrl);
        response.EnsureSuccessStatusCode();

        using Stream stream = await response.Content.ReadAsStreamAsync();
        return new Bitmap(stream);
    }
}
