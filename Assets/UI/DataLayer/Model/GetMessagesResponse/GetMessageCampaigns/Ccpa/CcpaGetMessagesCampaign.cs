using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CcpaGetMessagesCampaign : BaseGetMessagesCampaign
{
    [JsonInclude] public bool applies;
    [JsonInclude] public CcpaGetMessagesConsent userConsent;
    [JsonInclude] public BaseMessage message;
    public Dictionary<string, List<CmpUiElementModel>> ui;
    public Dictionary<string, string> popupBgColors;
}