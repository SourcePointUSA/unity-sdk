namespace UnityAppiumTests;

[TestFixture]
public class IosOnboardingTests
{
    [Test]
    public void ButtonAndAlertAreDismissedInOrder()
    {
        var actions = new List<string>();

        IosOnboarding.DismissIfPresent(
            () => () => actions.Add("button clicked"),
            () => () => actions.Add("alert accepted"));

        Assert.That(actions, Is.EqualTo(new[] { "button clicked", "alert accepted" }));
    }

    [Test]
    public void ButtonWithoutSystemAlertDoesNotBlockCmp()
    {
        var clicked = false;

        Assert.DoesNotThrow(() => IosOnboarding.DismissIfPresent(
            () => () => clicked = true,
            () => throw new NoAlertPresentException()));

        Assert.That(clicked, Is.True);
    }

    [Test]
    public void MissingButtonAndAlertDoNotBlockCmp()
    {
        Assert.DoesNotThrow(() => IosOnboarding.DismissIfPresent(
            () => throw new WebDriverTimeoutException("Onboarding button absent"),
            () => throw new WebDriverTimeoutException("System alert absent")));
    }

    [Test]
    public void SystemAlertCanBeDismissedWhenButtonIsAbsent()
    {
        var accepted = false;

        IosOnboarding.DismissIfPresent(
            () => throw new WebDriverTimeoutException("Onboarding button absent"),
            () => () => accepted = true);

        Assert.That(accepted, Is.True);
    }

    [Test]
    public void UnexpectedDriverFailureIsNotHidden()
    {
        Assert.Throws<WebDriverException>(() => IosOnboarding.DismissIfPresent(
            () => throw new WebDriverException("Session disconnected"),
            () => throw new AssertionException("Alert must not be checked after a driver failure")));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ActionTimeoutIsNotMistakenForMissingPrompt(bool buttonActionTimesOut)
    {
        Action timeout = () => throw new WebDriverTimeoutException("Visible prompt action timed out");
        Action success = () => { };

        Assert.Throws<WebDriverTimeoutException>(() => IosOnboarding.DismissIfPresent(
            () => buttonActionTimesOut ? timeout : success,
            () => buttonActionTimesOut ? success : timeout));
    }
}
