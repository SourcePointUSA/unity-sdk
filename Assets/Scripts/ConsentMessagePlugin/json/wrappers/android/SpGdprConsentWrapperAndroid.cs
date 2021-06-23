using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpGdprConsentWrapperAndroid : GdprConsentWrapper
    {
        // no TCdata here
        [JsonInclude] public object consentedToAll;
        [JsonInclude] public string addtlConsent;
        [JsonInclude] public string dateCreated;
        [JsonInclude] public string[] specialFeatures;
        [JsonInclude] public string[] acceptedVendors;
        [JsonInclude] public string[] legIntCategories;
        [JsonInclude] public string[] acceptedCategories;
    }

}