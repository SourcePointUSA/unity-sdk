using System.Collections.Generic;
using System.Text.Json.Serialization;

public class BaseMessage
{
    [JsonInclude] public int site_id;
    [JsonInclude] public List<MessageChoise> message_choice;
    [JsonInclude] public MessageJson message_json;
}