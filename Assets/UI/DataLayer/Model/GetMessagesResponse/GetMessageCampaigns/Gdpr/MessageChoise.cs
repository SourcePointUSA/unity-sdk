using System.Text.Json.Serialization;

public class MessageChoise
{
    [JsonInclude] public string button_text;
    [JsonInclude] public string iframe_url;
    [JsonInclude] public int choice_id;
    [JsonInclude] public int type;
}