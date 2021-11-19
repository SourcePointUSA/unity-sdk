using System.Text.Json.Serialization;

public class CmpSliderModel : CmpUiElementModel
{
    [JsonInclude] public CmpSliderSettingsModel settings;

    public string LeftText => settings?.leftText;
    public string RightText => settings?.rightText;
    public string BackgroundColor => settings?.style?.backgroundColor;
    public string ActiveBackgroundColor => settings?.style?.activeBackgroundColor;
    public ColoredFontModel DefaultFont => settings?.style?.font;
    public ColoredFontModel ActiveFont => settings?.style?.activeFont;
}

public class CmpSliderSettingsModel
{
    [JsonInclude] public string leftText;
    [JsonInclude] public string rightText;
    [JsonInclude] public CmpSliderStyleModel style;
}

public class CmpSliderStyleModel
{
    [JsonInclude] public string backgroundColor;
    [JsonInclude] public string activeBackgroundColor;
    [JsonInclude] public ColoredFontModel font;
    [JsonInclude] public ColoredFontModel activeFont;
}
