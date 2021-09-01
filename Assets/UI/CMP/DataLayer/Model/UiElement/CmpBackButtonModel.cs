using System.Text.Json.Serialization;

public class CmpBackButtonModel : CmpUiElementModel
{
    [JsonInclude] public CmpBackButtonSettingsModel settings;
    
    public string Text => settings?.text;
    public bool? StartFocus => settings?.startFocus;
    public string BackgroundColor => settings?.style?.backgroundColor;
    public ColoredFontModel Font => settings?.style?.font;
}

public class CmpBackButtonSettingsModel
{
    [JsonInclude] public CmpBackButtonStyleModel style;
    [JsonInclude] public string text;
    [JsonInclude] public bool startFocus;
}

public class CmpBackButtonStyleModel
{
    [JsonInclude] public string backgroundColor;
    [JsonInclude] public ColoredFontModel font;
}