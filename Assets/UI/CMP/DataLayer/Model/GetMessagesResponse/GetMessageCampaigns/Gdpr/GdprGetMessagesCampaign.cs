using System.Text.Json.Serialization;

public class GdprGetMessagesCampaign : BaseGetMessagesCampaign
{
    [JsonInclude] public bool applies;
    [JsonInclude] public GdprGetMessagesConsent userConsent;
    [JsonInclude] public GdprMessage message;
}