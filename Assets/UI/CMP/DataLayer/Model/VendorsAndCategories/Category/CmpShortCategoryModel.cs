using System.Text.Json.Serialization;

public class CmpShortCategoryModel
{
    [JsonInclude] public string _id;
    [JsonInclude] public string type;
    [JsonInclude] public string name;
    [JsonInclude] public string description;
}