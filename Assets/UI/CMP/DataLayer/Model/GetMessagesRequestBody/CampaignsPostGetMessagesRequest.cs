using System.Text.Json.Serialization;

public class CampaignsPostGetMessagesRequest
{
    [JsonInclude] public SingleCampaignPostGetMessagesRequest gdpr;
    [JsonInclude] public SingleCampaignPostGetMessagesRequest ccpa;

    public CampaignsPostGetMessagesRequest(SingleCampaignPostGetMessagesRequest gdpr, SingleCampaignPostGetMessagesRequest ccpa)
    {
        this.gdpr = gdpr;
        this.ccpa = ccpa;
    }
}