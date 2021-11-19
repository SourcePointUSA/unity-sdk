using System.Text.Json.Serialization;

public class ConsentCcpaSaveAndExitVariables : ConsentSaveAndExitVariables
{
    [JsonInclude] public ConsentGdprSaveAndExitVariablesCategory[] rejectedCategories;
    [JsonInclude] public ConsentGdprSaveAndExitVariablesVendor[] rejectedVendors;

    public ConsentCcpaSaveAndExitVariables(string language, string privacyManagerId, ConsentGdprSaveAndExitVariablesCategory[] rejectedCategories, ConsentGdprSaveAndExitVariablesVendor[] rejectedVendors, ConsentGdprSaveAndExitVariablesSpecialFeature[] specialFeatures)
    {
        this.lan = language;
        this.privacyManagerId = privacyManagerId;
        this.rejectedCategories = rejectedCategories;
        this.rejectedVendors = rejectedVendors;
        this.specialFeatures = specialFeatures;
    }
}