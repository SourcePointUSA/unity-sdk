using System.Text.Json.Serialization;

public class PostGetMessagesRequest
{
    [JsonInclude] public int accountId;
    [JsonInclude] public string propertyHref;
    [JsonInclude] public string idfaStatus;
    [JsonInclude] public string requestUUID;
    [JsonInclude] public CampaignsPostGetMessagesRequest campaigns;
    [JsonInclude] public LocalState localState;
    [JsonInclude] public IncludeDataPostGetMessagesRequest includeData;

    public PostGetMessagesRequest(int accountId, string propertyHref, string idfaStatus, string requestUUID, CampaignsPostGetMessagesRequest campaigns, LocalState localState, IncludeDataPostGetMessagesRequest includeData)
    {
        this.accountId = accountId;
        this.propertyHref = propertyHref;
        this.idfaStatus = idfaStatus;
        this.requestUUID = requestUUID;
        this.campaigns = campaigns;
        this.localState = localState;
        this.includeData = includeData;
    }
}