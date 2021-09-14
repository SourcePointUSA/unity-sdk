using System.Text.Json.Serialization;

public class GdprGetMessagesCampaign : BaseGetMessagesCampaign
{
    [JsonInclude] public bool applies;
    [JsonInclude] public string url;
    [JsonInclude] public GdprGetMessagesConsent userConsent;
    [JsonInclude] public GdprMessageMetaData messageMetaData;
    [JsonInclude] public GdprMessage message;
}