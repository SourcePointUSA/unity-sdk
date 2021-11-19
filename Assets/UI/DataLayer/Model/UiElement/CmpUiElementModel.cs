using System.Text.Json.Serialization;

public abstract class CmpUiElementModel
{
    [JsonInclude] public string id;
    [JsonInclude] public string type;
    [JsonInclude] public string name;
}