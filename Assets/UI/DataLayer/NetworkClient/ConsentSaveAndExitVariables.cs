using System.Text.Json.Serialization;

public class ConsentSaveAndExitVariables
{
    [JsonInclude] public string lan;
    [JsonInclude] public string privacyManagerId;
    [JsonInclude] public ConsentGdprSaveAndExitVariablesSpecialFeature[] specialFeatures;     
}