using System.Text.Json.Serialization;

public class ConsentGdprSaveAndExitVariablesVendor
{
    [JsonInclude] public string _id;
    [JsonInclude] public int? iabId;
    [JsonInclude] public string vendorType;
    [JsonInclude] public bool consent;
    [JsonInclude] public bool legInt;
}