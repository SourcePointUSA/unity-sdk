using System.Text.Json.Serialization;

public class CmpNativeButtonModel : CmpUiElementModel
{
    [JsonInclude] public CmpNativeButtonSettings settings;
    
    public string Text => settings?.text;
    public bool? StartFocus => settings?.startFocus;
    public string OnFocusBackgroundColor => settings?.style?.onFocusBackgroundColor;
    public string OnUnfocusBackgroundColor => settings?.style?.onUnfocusBackgroundColor;
    public string OnFocusTextColor => settings?.style?.onFocusTextColor;
    public string OnUnfocusTextColor => settings?.style?.onUnfocusTextColor;
    public FontModel Font => settings?.style?.font;
}

public class CmpNativeButtonSettings
{
    [JsonInclude] public CmpNativeButtonStyle style;
    [JsonInclude] public string text;
    [JsonInclude] public bool startFocus;
}

public class CmpNativeButtonStyle
{
    [JsonInclude] public string onFocusBackgroundColor;
    [JsonInclude] public string onUnfocusBackgroundColor;
    [JsonInclude] public string onFocusTextColor;
    [JsonInclude] public string onUnfocusTextColor;
    [JsonInclude] public FontModel font;
}