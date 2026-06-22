using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hearthstone_Archetype_Predictor.Models;

public class AzureMLClient
{
    private const string EndpointUrl =
        "https://projekt-endpoint.polandcentral.inference.ml.azure.com/score";

    private readonly string _apiKey;

    public AzureMLClient()
    {
        this._apiKey = AppConfig.AzureMLApiKey;
    }

    public async Task<string> PredictArchetypeAsync(string deckClass, List<int> cardDbfIds)
    {
        if (string.IsNullOrEmpty(this._apiKey))
            return "API key isn't set";

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        };

        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            this._apiKey
        );
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        string requestBody = BuildRequestBody(deckClass, cardDbfIds);
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client
            .PostAsync(EndpointUrl, content)
            .ConfigureAwait(false);

        if (response.IsSuccessStatusCode is false)
        {
            string errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Azure ML endpoint returned an error {response.StatusCode}: {errorBody}"
            );
        }

        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return ParseArchetype(result);
    }

    private static string BuildRequestBody(string deckClass, List<int> cardDbfIds)
    {
        var sorted = cardDbfIds.OrderBy(id => id).ToList();
        while (sorted.Count < 30)
            sorted.Add(0);
        var trimmed = sorted.Take(30).ToList();

        var columns = new List<string> { "deck_class" };
        for (int i = 0; i < 30; i++)
            columns.Add($"card_{i}");

        var row = new List<object> { deckClass };
        row.AddRange(trimmed.Cast<object>());

        var payload = new
        {
            input_data = new
            {
                columns,
                index = new[] { 0 },
                data = new[] { row },
            },
            @params = new { },
        };

        return JsonSerializer.Serialize(payload);
    }

    private static string ParseArchetype(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var first = root[0];
                if (first.ValueKind == JsonValueKind.String)
                    return first.GetString() ?? "Unknown";

                if (first.TryGetProperty("prediction", out var prediction))
                    return prediction.GetString() ?? "Unknown";
                if (first.TryGetProperty("result", out var result))
                    return first.GetString() ?? "Unknown";
            }

            return json.Trim('"', '[', ']');
        }
        catch
        {
            return "Unknown";
        }
    }
}
