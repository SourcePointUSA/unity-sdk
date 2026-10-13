namespace UnityAppiumTests;

[TestFixture]
public class AndroidLocatorTests
{
    [Test]
    public void ButtonWithLabelMatchesTextAndContentDescription()
    {
        Assert.That(AndroidLocator.ButtonWithLabel("Accept All"), Is.EqualTo(
            "//android.widget.Button[@text='Accept All' or @content-desc='Accept All']"));
    }

    [Test]
    public void ElementWithTextMatchesAnyAndroidViewClass()
    {
        Assert.That(AndroidLocator.ElementWithText("USNat Message"), Is.EqualTo(
            "//*[@text='USNat Message']"));
    }

    [Test]
    public void ButtonWithLabelSupportsPrivacyManagerActions()
    {
        Assert.That(AndroidLocator.ButtonWithLabel("Save & Exit"), Is.EqualTo(
            "//android.widget.Button[@text='Save & Exit' or @content-desc='Save & Exit']"));
    }

    [Test]
    public void PrivacyManagerSwitchMatchesCurrentAndroidSwitchClass()
    {
        Assert.That(AndroidLocator.SwitchAt(1), Is.EqualTo("(//android.widget.Switch)[1]"));
    }

    [Test]
    public void PrivacyManagerToggleMatchesItsPurposeLabel()
    {
        Assert.That(AndroidLocator.PrivacyManagerToggle("Store and/or access information on a device"), Is.EqualTo(
            "//android.widget.ToggleButton[@text='Store and/or access information on a device']"));
    }

    [Test]
    public void PrivacyManagerCategoryToggleMatchesTheCategoryRow()
    {
        Assert.That(AndroidLocator.PrivacyManagerCategoryToggle("Category 2"), Is.EqualTo(
            "//android.view.View[android.widget.TextView[@text='Category 2']]/android.widget.ToggleButton"));
    }
}
