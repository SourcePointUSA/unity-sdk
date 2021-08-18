using System.Text.Json.Serialization;

public class FontModel
{
    [JsonInclude] public int fontSize;
    [JsonInclude] public string fontWeight;
    [JsonInclude] public string fontFamily;
}

public class ColoredFontModel : FontModel
{
    [JsonInclude] public string color;
    public string Color => color;
}