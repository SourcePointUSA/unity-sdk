using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
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
}