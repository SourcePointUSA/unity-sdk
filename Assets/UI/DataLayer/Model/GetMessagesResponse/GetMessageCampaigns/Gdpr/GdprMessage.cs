using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GdprMessage : BaseMessage
{
   [JsonInclude] public string language;
   [JsonInclude] public List<CmpShortCategoryModel> categories;
}