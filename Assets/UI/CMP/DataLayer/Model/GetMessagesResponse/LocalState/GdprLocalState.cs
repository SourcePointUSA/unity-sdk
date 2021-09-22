using System.Text.Json.Serialization;

public class GdprLocalState : Ios14LocalState
{
    [JsonInclude] public string uuid;
    [JsonInclude] public int propertyId;
    [JsonInclude] public int messageId;
    [JsonInclude] public string[] mmsCookies;

}