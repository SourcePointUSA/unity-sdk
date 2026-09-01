namespace UnityAppiumTests;

public static class AndroidLocator
{
    public static string ButtonWithLabel(string label) =>
        $"//android.widget.Button[@text='{label}' or @content-desc='{label}']";
}
