using System.Text.Json.Serialization;

public class ConsentGdprSaveAndExitVariablesCategory
{
    [JsonInclude] public string _id;
    [JsonInclude] public int iabId;
    [JsonInclude] public string type;
    [JsonInclude] public bool consent;
    [JsonInclude] public bool legInt;
}