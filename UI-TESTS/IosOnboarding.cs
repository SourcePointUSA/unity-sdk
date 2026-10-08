namespace UnityAppiumTests;

internal static class IosOnboarding
{
    internal static void DismissIfPresent(Action clickButton, Action acceptAlert)
    {
        try
        {
            clickButton();
        }
        catch (WebDriverTimeoutException)
        {
            // The onboarding button is absent after its bounded discovery wait.
        }

        try
        {
            acceptAlert();
        }
        catch (WebDriverTimeoutException)
        {
            // iOS may preserve permission decisions across app reinstalls.
        }
        catch (NoAlertPresentException)
        {
            // The system alert can disappear between discovery and acceptance.
        }
    }
}
