using System.Collections.Generic;
using System.Text.Json.Serialization;

public class MessageJsonSettings
{
    [JsonInclude] public Dictionary<string, MessageJsonSettingsSingleLanguage> languages;
    [JsonInclude] public string iframeTitle;
    [JsonInclude] public SelectedPrivacyManager selected_privacy_manager;
}