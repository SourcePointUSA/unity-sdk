using System.Collections.Generic;
using System.Text.Json.Serialization;

public class IncludeDataPostGetMessagesRequest
{
    [JsonInclude] public Dictionary<string, string> localState;
    [JsonInclude] public Dictionary<string, string> TCData;
    [JsonInclude] public Dictionary<string, string> messageMetaData;
}