using System.Text.Json.Serialization;

public class ConsentGdprSaveAndExitVariables
{
    [JsonInclude] public string lan;
    [JsonInclude] public string privacyManagerId;
    [JsonInclude] public ConsentGdprSaveAndExitVariablesCategory[] categories;
    [JsonInclude] public ConsentGdprSaveAndExitVariablesVendor[] vendors;
    [JsonInclude] public ConsentGdprSaveAndExitVariablesSpecialFeature[] specialFeatures;     

    public ConsentGdprSaveAndExitVariables(string language, string privacyManagerId, ConsentGdprSaveAndExitVariablesCategory[] categories, ConsentGdprSaveAndExitVariablesVendor[] vendors, ConsentGdprSaveAndExitVariablesSpecialFeature[] specialFeatures)
    {
        this.lan = language;
        this.privacyManagerId = privacyManagerId;
        this.categories = categories;
        this.vendors = vendors;
        this.specialFeatures = specialFeatures;
    }
}