using System.Text.Json.Serialization;

public class GdprMessageJsonSettings
{
    [JsonInclude] public GdprMessageJsonSettingsLanguages languages;
    [JsonInclude] public string iframeTitle;
    [JsonInclude] public SelectedPrivacyManager selected_privacy_manager;
}