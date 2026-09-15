namespace UnityAppiumTests;

public static class AndroidLocator
{
    public static string ButtonWithLabel(string label) =>
        $"//android.widget.Button[@text='{label}' or @content-desc='{label}']";

    public static string ElementWithText(string text) =>
        $"//*[@text='{text}']";

    public static string SwitchAt(int index) =>
        $"(//android.widget.Switch)[{index}]";
}
