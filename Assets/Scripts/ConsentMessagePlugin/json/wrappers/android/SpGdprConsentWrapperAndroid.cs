using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpGdprConsentWrapperAndroid : GdprConsentWrapper
    {
        // no TCFdata here
        [JsonInclude] public object consentedToAll;
        [JsonInclude] public string addtlConsent;
        [JsonInclude] public string dateCreated;
        [JsonInclude] public string[] specialFeatures;
        [JsonInclude] public string[] acceptedVendors;
        [JsonInclude] public string[] legIntCategories;
        [JsonInclude] public string[] acceptedCategories;
    }

}