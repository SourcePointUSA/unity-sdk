using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GdprMessage
{
    [JsonInclude] public int site_id;
    [JsonInclude] public string language;
    [JsonInclude] public List<CmpShortCategoryModel> categories;
    [JsonInclude] public List<GdprMessageChoise> message_choice;
    [JsonInclude] public GdprMessageJson message_json;
}