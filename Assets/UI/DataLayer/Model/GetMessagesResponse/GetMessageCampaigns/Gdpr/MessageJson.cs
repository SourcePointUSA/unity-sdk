using System.Collections.Generic;
using System.Text.Json.Serialization;

public class MessageJson
{
    [JsonInclude] public string type;
    [JsonInclude] public string name;
    [JsonInclude] public MessageJsonSettings settings;
    [JsonInclude] public bool compliance_status;
    // [JsonInclude] public KeyValuePair<string,bool>[] compliance_list;    //TODO?
    // [JsonInclude] public List<MessageJsonUi> children;
}