using Hearthstone_Archetype_Predictor.Models;
using Hearthstone_Archetype_Predictor.Tests.Helpers;
using Hearthstone_Archetype_Predictor.Tests.PageObjects;
using Xunit;

namespace Hearthstone_Archetype_Predictor.Tests;

public class HearthstoneSerializerIntegrationTests
{
    private const string WarriorDeckstring =
        "AAECAQcCrwSRvAIOHLACkQP/A44FqAXUBaQG7gbnB+8HgrACiLACub8CAAA=";

    private const string DruidDeckstring =
        "AAECAbSKAwrbnwSboASjoAS4oASD1ASY1ASZ1ASh1ASxmAX9xAUKh58EiZ8Erp8E2Z8E2p8EuaAE3KAEgdQEo9QEjaQFAAEB8L8E/cQFAdqhBf3EBQA=";

    [Fact]
    public async Task IsValidDeckstring_WarriorDeckstring_PassesDeserialization()
    {
        //Given
        var page = new SerializerPage(WarriorDeckstring);
        //When
        await page.WaitForNetworkDelay(100);
        bool result = page.SubmitDeckString(WarriorDeckstring);
        //Then
        Assert.True(result, "Warrior deck isn't valid");
    }

    [Fact]
    public async Task IsValidDeckString_ChangingDeckstring_BothPassValidation()
    {
        //Given
        var page = new SerializerPage(WarriorDeckstring);
        //When
        bool result = page.SubmitDeckString(WarriorDeckstring);
        //Then
        Assert.True(result, "Warrior deck isn't valid");

        await WaitHelper.WaitUntil(
            condition: () => page.CurrentDeckString == WarriorDeckstring,
            timeout: TimeSpan.FromSeconds(1)
        );
        //When
        result = page.SubmitDeckString(DruidDeckstring);
        //Then
        Assert.True(result, "Druid deck isn't valid");
        Assert.Equal(DruidDeckstring, page.CurrentDeckString);
    }

    [Fact]
    public void IsValidDeckString_InvalidDeckstring_FailsDeserialization()
    {
        //Given
        var page = new SerializerPage();
        const string InvalidDeckstring =
            "AAECAQcCrwSRvAIOHLACkQP/A44FqAXUBaQG7gbnB+8HgrACiLACub8CAAA=ddas";
        //When
        bool result = page.SubmitDeckString(InvalidDeckstring);
        //Then
        Assert.False(result, "Invalid deck string should have failed deserialization.");
    }
}
