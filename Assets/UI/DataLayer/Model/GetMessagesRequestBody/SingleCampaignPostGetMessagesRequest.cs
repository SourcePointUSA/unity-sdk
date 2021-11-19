using System.Collections.Generic;
using System.Text.Json.Serialization;

public class SingleCampaignPostGetMessagesRequest
{
    [JsonInclude] public Dictionary<string, string> targetingParams;

    public SingleCampaignPostGetMessagesRequest(Dictionary<string, string> targetingParams)
    {
        this.targetingParams = targetingParams;
    }
}