using System.Text.Json.Serialization;

public class CmpVendorCategoryModel
{
    [JsonInclude] public string type;
    [JsonInclude] public int? iabId;
    [JsonInclude] public string name;
}