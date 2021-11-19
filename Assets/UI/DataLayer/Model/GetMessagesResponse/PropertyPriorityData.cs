using System.Collections.Generic;
using System.Text.Json.Serialization;

public class PropertyPriorityData
{
    [JsonInclude] public int stage_message_limit;
    [JsonInclude] public int site_id;
    [JsonInclude] public bool multi_campaign_enabled;
    [JsonInclude] public int public_message_limit;
    [JsonInclude] public List<int> public_campaign_type_priority;
    [JsonInclude] public List<int> stage_campaign_type_priority;
}