using System.Collections.Generic;
using System.Text.Json.Serialization;

public class MessageJsonUiSubelementSettings
{
    [JsonInclude] public Dictionary<string, MessageJsonUiSettingsLanguage> languages;
    [JsonInclude] public string text;
    [JsonInclude] public MessageJsonUiSubelementSettingsChoiceOption choice_option;
    [JsonInclude] public string background;
    [JsonInclude] public string action_type;
    [JsonInclude] public ColoredFontModel font;
}