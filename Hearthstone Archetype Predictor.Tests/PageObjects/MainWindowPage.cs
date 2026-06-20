using System.Collections.ObjectModel;
using Hearthstone_Archetype_Predictor.Tests.Helpers;
using Hearthstone_Archetype_Predictor.ViewModels;

namespace Hearthstone_Archetype_Predictor.Tests.PageObjects;

/// <summary>
/// Page Object Model for the application's main window.
///
/// The POM pattern (from the Selenium world) encapsulates interaction with a single “screen”.
/// Instead of tests calling ViewModel methods directly, they use this class —
/// which makes tests more readable and easier to maintain.
///
/// Selenium equivalent:
///   public class LoginPage {
///       private IWebDriver driver;
///       public void EnterUsername(string u) => driver.FindElement(By.Id("user")).SendKeys(u);
///   }
///
/// Here:
///   No driver; we use the ViewModel directly.
///   No DOM elements; we use ViewModel properties and methods.
/// </summary>
public class MainWindowPage
{
    private readonly MainWindowViewModel _vm;

    public MainWindowPage(double windowWidth = 900, double windowHeight = 850)
    {
        this._vm = new MainWindowViewModel();
        this._vm.WindowHeight = windowHeight;
        this._vm.WindowWidth = windowWidth;
    }

    /// <summary>
    /// Number of loaded card images.
    /// </summary>
    public int ImageCount => _vm.Images.Count;

    /// <summary>
    /// Lists "X\t Card name" strings.
    /// </summary>
    public ObservableCollection<string> CardNamesAndCopies => _vm.CardNamesAndCopies;

    /// <summary>
    /// Current window scale factor
    /// </summary>
    public double Scale => _vm.Scale;

    public double FontSizeTitle => _vm.FontSizeTitle;
    public double FontSizeNormal => _vm.FontSizeNormal;
    public double ImageWidth => _vm.ImageWidth;
    public double ImageHeight => _vm.ImageHeight;

    /// <summary>
    /// Simulates input of a new deck string
    /// Returns true if the deck is valid
    /// </summary>
    public Task<bool> EnterDeckstring(string deckstring) => _vm.NewDeckstring(deckstring);

    /// <summary>
    /// Changed the window size (like resize in browser)
    /// </summary>
    public void ResizeWindow(double width, double height)
    {
        _vm.WindowWidth = width;
        _vm.WindowHeight = height;
    }

    /// <summary>
    /// Waits until the cards load (ImageCount > 0), with timeout
    /// Equivalent: wait.Until()
    /// </summary>
    public Task WaitForImagesLoaded(TimeSpan? timeout = null) =>
        WaitHelper.WaitUntil(
            condition: () => ImageCount > 0,
            timeout: timeout ?? TimeSpan.FromSeconds(10),
            timeoutMessage: "Card images aren't loaded within the specified timeout."
        );

    /// <summary>
    /// Wait until CardNamesAndCopies list is filled.
    /// </summary>
    public Task WaitForCardNamesLoaded(TimeSpan? timeout = null) =>
        WaitHelper.WaitUntil(
            condition: () => CardNamesAndCopies.Count > 0,
            timeout: timeout ?? TimeSpan.FromSeconds(10),
            timeoutMessage: "Card names aren't loaded within the specified timeout."
        );
}
