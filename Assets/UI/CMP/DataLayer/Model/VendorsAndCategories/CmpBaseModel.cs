using System.Text.Json.Serialization;

public class CmpBaseModel
{
    [JsonInclude] public string _id;
    [JsonInclude] public int/*?*/ iabId;
    [JsonInclude] public string name;
}
