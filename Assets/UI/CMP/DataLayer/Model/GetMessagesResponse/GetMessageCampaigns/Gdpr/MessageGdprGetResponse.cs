using System.Collections.Generic;
using System.Text.Json.Serialization;

public class MessageGdprGetResponse
{
    [JsonInclude] public GdprMessage message;
    [JsonInclude] public MessageMetaData messageMetaData;
    public Dictionary<string, List<CmpUiElementModel>> ui;
    public Dictionary<string, string> popupBgColors;
}