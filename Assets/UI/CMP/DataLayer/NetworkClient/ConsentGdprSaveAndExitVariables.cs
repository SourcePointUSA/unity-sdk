using System.Text.Json.Serialization;

public class ConsentGdprSaveAndExitVariables : ConsentSaveAndExitVariables
{
    [JsonInclude] public ConsentGdprSaveAndExitVariablesCategory[] categories;
    [JsonInclude] public ConsentGdprSaveAndExitVariablesVendor[] vendors;

    public ConsentGdprSaveAndExitVariables(string language, string privacyManagerId, ConsentGdprSaveAndExitVariablesCategory[] categories, ConsentGdprSaveAndExitVariablesVendor[] vendors, ConsentGdprSaveAndExitVariablesSpecialFeature[] specialFeatures)
    {
        this.lan = language;
        this.privacyManagerId = privacyManagerId;
        this.categories = categories;
        this.vendors = vendors;
        this.specialFeatures = specialFeatures;
    }
}