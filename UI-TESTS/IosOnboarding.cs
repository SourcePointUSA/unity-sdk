namespace UnityAppiumTests;

internal static class IosOnboarding
{
    internal static void DismissIfPresent(Func<Action> findButtonAction, Func<Action> findAlertAction)
    {
        Action? clickButton = null;
        try
        {
            clickButton = findButtonAction();
        }
        catch (WebDriverTimeoutException)
        {
            // The onboarding button is absent after its bounded discovery wait.
        }
        clickButton?.Invoke();

        Action? acceptAlert = null;
        try
        {
            acceptAlert = findAlertAction();
        }
        catch (WebDriverTimeoutException)
        {
            // iOS may preserve permission decisions across app reinstalls.
        }
        catch (NoAlertPresentException)
        {
            // No system alert is available during discovery.
        }
        acceptAlert?.Invoke();
    }
}
