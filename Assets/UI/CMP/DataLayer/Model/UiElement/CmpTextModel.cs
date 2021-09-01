using System.Text.Json.Serialization;

public class CmpTextModel : CmpUiElementModel
{
    [JsonInclude] public CmpTextSettingsModel settings;
    public string Text => settings?.text;
    public ColoredFontModel Font => settings?.style?.font;
}

public class CmpTextSettingsModel
{
    [JsonInclude] public string text;
    [JsonInclude] public CmpTextStyleModel style;
}

public class CmpTextStyleModel
{
    [JsonInclude] public ColoredFontModel font;
}