using System.Text.Json.Serialization;

public class GdprLocalState : Ios14LocalState
{
    [JsonInclude] public string uuid;
    [JsonInclude] public int messageId;
}