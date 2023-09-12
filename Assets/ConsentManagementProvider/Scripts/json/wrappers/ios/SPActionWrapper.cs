using System.Text.Json.Serialization;

public class SpActionWrapper
    {
        [JsonInclude] public string type;
        [JsonInclude] public string customActionId;
    }