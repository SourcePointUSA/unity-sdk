using System.Text.Json.Serialization;

public class ConsentGdprSaveAndExitVariables
{
    [JsonInclude] public string lan;
    [JsonInclude] public string privacyManagerId;
    [JsonInclude] public ConsentGdprSaveAndExitVariablesCategory[] categories;
    [JsonInclude] public ConsentGdprSaveAndExitVariablesVendor[] vendors;
    // [JsonInclude] public [] specialFeatures;
}