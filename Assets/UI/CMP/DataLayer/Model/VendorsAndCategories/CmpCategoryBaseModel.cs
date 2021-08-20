using System.Text.Json.Serialization;

public class CmpCategoryBaseModel : CmpBaseModel
{
    [JsonInclude] public string description;
}
