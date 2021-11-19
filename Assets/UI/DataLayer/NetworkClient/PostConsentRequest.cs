using System.Text.Json.Serialization;

public class PostConsentRequest
{
    // [JsonInclude] public List<string> pubData = new List<string>();                //TODO
    [JsonInclude] public IncludeDataPostGetMessagesRequest includeData;
    [JsonInclude] public string requestUUID;
    [JsonInclude] public string idfaStatus;
    [JsonInclude] public LocalState localState;
}