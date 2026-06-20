using System;
using System.Threading.Tasks;

namespace Hearthstone_Archetype_Predictor.Tests.Helpers;

/// <summary>
/// Equivalent of Selenium WebDriver Wait commands for desktop/unit tests.
///
/// Selenium provides:
///   - ImplicitWait  - waits a fixed amount of time
///   - ExplicitWait - waits until a condition is met (WebDriverWait + ExpectedConditions)
///   - FluentWait - polling with ignored exceptions
///
/// This class maps the same concepts to async C# tests.
/// </summary>
public static class WaitHelper
{
    /// <summary>
    /// Implicit wait — waits for a fixed number of milliseconds.
    /// Equivalent to: driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(ms);
    /// </summary>
    public static Task ImplicitWait(int milliseconds = 500) => Task.Delay(milliseconds);

    /// <summary>
    /// Explicit wait — waits until <paramref name="condition"/> returns true,
    /// or until <paramref name="timeout"/> expires.
    /// Equivalent to: new WebDriverWait(driver, timeout).Until(condition)
    /// </summary>
    /// <exception cref="TimeoutException">If the condition is not met in time.</exception>
    public static async Task WaitUntil(
        Func<bool> condition,
        TimeSpan? timeout = null,
        int pollIntervalMilliseconds = 100,
        string? timeoutMessage = null
    )
    {
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(5));

        while (DateTime.UtcNow < deadline)
        {
            if (condition())
                return;

            await Task.Delay(pollIntervalMilliseconds);
        }

        throw new TimeoutException(
            timeoutMessage ?? $"Condition wasn't fulfilled between {timeout?.TotalSeconds ?? 5}s."
        );
    }

    /// <summary>
    /// Fluent wait — like WaitUntil, but ignores specified exceptions during polling.
    /// Equivalent to: FluentWait with .IgnoreExceptionTypes(...)
    /// </summary>
    public static async Task<T> FluentWait<T>(
        Func<T?> supplier,
        Predicate<T?> until,
        TimeSpan? timeout = null,
        int pollIntervalMilliseconds = 100,
        string? timeoutMessage = null
    )
        where T : class
    {
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(5));

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var result = supplier();
                if (result is not null && until(result))
                    return result;
            }
            catch (Exception e) { }

            await Task.Delay(pollIntervalMilliseconds);
        }

        throw new TimeoutException(
            timeoutMessage ?? $"FluentWait wasn't fulfilled between {timeout?.TotalSeconds ?? 5}s."
        );
    }
}
