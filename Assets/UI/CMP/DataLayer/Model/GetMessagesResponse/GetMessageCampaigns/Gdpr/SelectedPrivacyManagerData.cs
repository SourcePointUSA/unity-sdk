using System.Text.Json.Serialization;

public class SelectedPrivacyManagerData
{
    [JsonInclude] public string button_text;
    [JsonInclude] public string privacy_manager_iframe_url;
    [JsonInclude] public string consent_origin;
}