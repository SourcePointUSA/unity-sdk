using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class GdprConsentWrapper
    {
        [JsonInclude] public string uuid;
        [JsonInclude] public string euconsent;
        [JsonInclude] public Dictionary<string, object> TCData;
        [JsonInclude] public Dictionary<string, SpVendorGrantWrapper> grants;
        [JsonInclude] public List<string> acceptedCategories;
        [JsonInclude] public bool applies;
        [JsonInclude] public string webConsentPayload;
        [JsonInclude] public ConsentStatusWrapper consentStatus;
    }

    internal class ConsentStatusWrapper
    {
        [JsonInclude] public bool? rejectedAny;
        [JsonInclude] public bool? rejectedLI;
        [JsonInclude] public bool? consentedAll;
        [JsonInclude] public bool? consentedToAny;
        [JsonInclude] public bool? vendorListAdditions;
        [JsonInclude] public bool? legalBasisChanges;
        [JsonInclude] public GranularStatusWrapper? granularStatus;
        [JsonInclude] public bool hasConsentData;
        [JsonInclude] public object rejectedVendors;
        [JsonInclude] public object rejectedCategories;
    }

    internal class GranularStatusWrapper
    {
        [JsonInclude] public string? vendorConsent;
        [JsonInclude] public string? vendorLegInt;
        [JsonInclude] public string? purposeConsent;
        [JsonInclude] public string? purposeLegInt;
        [JsonInclude] public bool? previousOptInAll;
        [JsonInclude] public bool? defaultConsent;
    }
}