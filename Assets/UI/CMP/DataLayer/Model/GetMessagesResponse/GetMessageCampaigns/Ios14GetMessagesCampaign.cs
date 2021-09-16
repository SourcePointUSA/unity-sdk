using System.Text.Json.Serialization;

public class Ios14GetMessagesCampaign : BaseGetMessagesCampaign
{
    [JsonInclude] public BaseMessage message;
}