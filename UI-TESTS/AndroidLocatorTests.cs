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
}
