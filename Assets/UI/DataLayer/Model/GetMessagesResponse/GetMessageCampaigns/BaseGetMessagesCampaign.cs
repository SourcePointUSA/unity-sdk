using System.Text.Json.Serialization;

public class BaseGetMessagesCampaign
{
    [JsonInclude] public string type;
    [JsonInclude] public MessageMetaData messageMetaData;
    [JsonInclude] public string url;
}