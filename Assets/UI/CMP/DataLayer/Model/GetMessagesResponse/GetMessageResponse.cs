using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GetMessageResponse
{
    [JsonInclude] public int propertyId;
    [JsonInclude] public PropertyPriorityData propertyPriorityData;
    [JsonInclude] public LocalState localState;
    [JsonInclude] public List<BaseGetMessagesCampaign> campaigns;
}