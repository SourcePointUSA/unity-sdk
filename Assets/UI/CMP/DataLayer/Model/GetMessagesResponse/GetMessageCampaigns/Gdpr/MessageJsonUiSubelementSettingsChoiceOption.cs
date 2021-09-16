using System.Text.Json.Serialization;

public class MessageJsonUiSubelementSettingsChoiceOption
{
    [JsonInclude] public int type;
    [JsonInclude] public MessageJsonUiSubelementSettingsChoiceOptionData data;
}