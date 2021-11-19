using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GdprGetMessagesCampaign : BaseGetMessagesCampaign
{
    [JsonInclude] public bool applies;
    [JsonInclude] public GdprGetMessagesConsent userConsent;
    [JsonInclude] public GdprMessage message;
    public Dictionary<string, List<CmpUiElementModel>> ui;
    public Dictionary<string, string> popupBgColors;
}