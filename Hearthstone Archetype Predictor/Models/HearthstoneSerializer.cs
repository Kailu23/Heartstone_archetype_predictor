using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using HearthDb.Deckstrings;

namespace Hearthstone_Archetype_Predictor.Models;

public class HearthstoneSerializer
{
    private string _deckString;
    public string DeckString
    {
        get { return _deckString; }
        set { _deckString = value; }
    }
    private Deck _deck;

    private Dictionary<HearthDb.Card, int> _cards;
    public Dictionary<HearthDb.Card, int> Cards
    {
        get { return this._cards; }
    }

    private List<Bitmap> _images;

    private Dictionary<HearthDb.Card, string> _imageUrls;

    private string baseUrl = "https://art.hearthstonejson.com/v1/render/latest/enUS/512x";

    public HearthstoneSerializer(
        string deckString = "AAECAQcCrwSRvAIOHLACkQP/A44FqAXUBaQG7gbnB+8HgrACiLACub8CAAA="
    )
    {
        this._deckString = deckString;
        this._deck = new Deck();
        this._cards = new Dictionary<HearthDb.Card, int>();
        this._images = new List<Bitmap>();
        this._imageUrls = new Dictionary<HearthDb.Card, string>();
    }

    public async Task<List<Bitmap>> GetImagesAsync()
    {
        await Deserialize(this._deckString);
        return _images;
    }

    private async Task Deserialize(string deckString)
    {
        List<Bitmap> downloadedBitmaps = new List<Bitmap>();
        this._deck = DeckSerializer.Deserialize(deckString);

        _cards = _deck.GetCards();

        _imageUrls.Clear();
        _images.Clear();
        string imageUrl;
        foreach (var card in _cards)
        {
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
    private string CreateImageUrl(string id)
    {
        return $"{baseUrl}/{id}.png";
    }

    private async Task<List<Bitmap>> DownloadImagesAsync(
        Dictionary<HearthDb.Card, string> imageUrls
    )
    {
        try
        {
            var downloadTasks = imageUrls.Select(url => DownloadImageAsync(url.Value));

            var bitmaps = await Task.WhenAll(downloadTasks);

            return bitmaps.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading images: {ex.Message}");
            return new List<Bitmap>();
        }
    }

    private async Task<Bitmap> DownloadImageAsync(string imageUrl)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(imageUrl);
        response.EnsureSuccessStatusCode();

        using Stream stream = await response.Content.ReadAsStreamAsync();
        return new Bitmap(stream);
    }

    public bool IsValidDeckString(string deckString)
    {
        if (string.IsNullOrWhiteSpace(deckString))
            return false;

        try
        {
            Deck deck = DeckSerializer.Deserialize(deckString);
            return deck is not null && deck.GetCards().Count > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the hero class and a list of all card DbfIds, sorted in ascending order.
    /// Cards with copies (2x) appear twice in the list — just like the Python example shows.
    /// Used for sending data to the Azure ML endpoint.
    /// </summary>
    public (string DeckClass, List<int> CardDbfIds) GetDeckClassAndCardIds()
    {
        if (this._deck is null || _cards.Count == 0)
            throw new InvalidOperationException(
                "Deck isn't deserialized. Call GetImagesAsync() first."
            );

        string rawClass = _cards.Keys.First().Class.ToString();
        string heroClass = char.ToUpper(rawClass[0]) + rawClass[1..].ToLower();

        var ids = _cards
            .SelectMany(kvp => Enumerable.Repeat(kvp.Key.DbfId, kvp.Value))
            .OrderBy(id => id)
            .ToList();

        return (heroClass, ids);
    }

    /// <summary>
    /// Accepts or cleans either a deck string or a full Hearthstone export text.
    /// The deck string is the first line that does not start with '#' and is not empty.
    /// </summary>
    public static string ExtractDeckString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        foreach (string line in input.Split('\n'))
        {
            string trimmed = line.Trim();
            if (trimmed.Length > 0 && !trimmed.StartsWith('#'))
                return trimmed;
        }
        return string.Empty;
    }
}
