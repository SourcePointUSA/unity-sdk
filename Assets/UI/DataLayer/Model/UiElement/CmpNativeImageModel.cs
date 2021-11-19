using System.Text.Json.Serialization;

public class CmpNativeImageModel : CmpUiElementModel
{
    [JsonInclude] public CmpNativeImageSettingsModel settings;

    public string LogoImageLink => settings?.src;
}

public class CmpNativeImageSettingsModel
{
    [JsonInclude] public string src;
}