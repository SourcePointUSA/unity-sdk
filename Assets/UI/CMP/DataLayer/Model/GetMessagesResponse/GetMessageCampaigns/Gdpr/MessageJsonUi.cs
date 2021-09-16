using System.Text.Json.Serialization;

public class MessageJsonUi
{
    [JsonInclude] public string type;
    [JsonInclude] public string name;
    [JsonInclude] public MessageJsonUiSubelementSettings settings;
    // [JsonInclude] public List<GdprMessageJsonUiSubelement> children;
}