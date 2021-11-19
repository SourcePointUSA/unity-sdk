using System.Text.Json.Serialization;

public class MessageJsonUiSubelementSettingsChoiceOptionData
{
    [JsonInclude] public string button_text;
    [JsonInclude] public string consent_origin;
    [JsonInclude] public string consent_language;
}