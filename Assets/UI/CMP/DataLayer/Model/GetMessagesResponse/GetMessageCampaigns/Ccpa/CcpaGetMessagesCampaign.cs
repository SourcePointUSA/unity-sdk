using System.Text.Json.Serialization;

public class CcpaGetMessagesCampaign : BaseGetMessagesCampaign
{
    [JsonInclude] public bool applies;
    [JsonInclude] public CcpaGetMessagesConsent userConsent;
    [JsonInclude] public BaseMessage message;
}