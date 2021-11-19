using System.Text.Json.Serialization;

public class CmpLongButtonModel : CmpUiElementModel
{
    [JsonInclude] public CmpLongButtonSettingsModel settings;

    public string OnText => settings?.onText;
    public string OffText => settings?.offText;
    public string CustomText => settings?.customText;
    public string OnFocusColorCode => settings?.style?.onFocusBackgroundColor;
    public string OnUnfocusColorCode => settings?.style?.onUnfocusBackgroundColor;
    public ColoredFontModel Font => settings?.style?.font;
    //public bool StartFocus => settings?.startFocus;   
}

public class CmpLongButtonSettingsModel
{
    [JsonInclude] public string onText;
    [JsonInclude] public string offText;
    [JsonInclude] public string customText;
    [JsonInclude] public CmpLongButtonStyleModel style;
    //[JsonInclude] public bool startFocus;
}

public class CmpLongButtonStyleModel
{
    [JsonInclude] public string onFocusBackgroundColor;
    [JsonInclude] public string onUnfocusBackgroundColor;
    [JsonInclude] public ColoredFontModel font;
}