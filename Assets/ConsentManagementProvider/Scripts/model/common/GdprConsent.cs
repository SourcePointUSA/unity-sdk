using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class GdprConsent
    {
        [JsonInclude] public bool applies;
        [JsonInclude] public string uuid;
        [JsonInclude] public string webConsentPayload;
        [JsonInclude] public string euconsent;
        [JsonInclude] public Dictionary<string, object> TCData;
        [JsonInclude] public Dictionary<string, SpVendorGrant> grants;
        [JsonInclude] public ConsentStatus consentStatus;
    }
    
    public class ConsentStatus
    {
        [JsonInclude] public bool? rejectedAny;
        [JsonInclude] public bool? rejectedLI;
        [JsonInclude] public bool? consentedAll;
        [JsonInclude] public bool? consentedToAny;
        [JsonInclude] public bool? vendorListAdditions;
        [JsonInclude] public bool? legalBasisChanges;
        [JsonInclude] public bool hasConsentData;
        [JsonInclude] public GranularStatus? granularStatus;
        [JsonInclude] public object rejectedVendors;
        [JsonInclude] public object rejectedCategories;

        public ConsentStatus(bool? rejectedAny, bool? rejectedLI, bool? consentedAll, 
            bool? consentedToAny, bool? vendorListAdditions, bool? legalBasisChanges, 
            bool? hasConsentData, GranularStatus? granularStatus, object rejectedVendors, object rejectedCategories)
        {
            this.rejectedAny = rejectedAny;
            this.rejectedLI = rejectedLI;
            this.consentedAll = consentedAll;
            this.consentedToAny = consentedToAny;
            this.vendorListAdditions = vendorListAdditions;
            this.legalBasisChanges = legalBasisChanges;
            this.hasConsentData = hasConsentData ?? false;
            this.granularStatus = granularStatus;
            this.rejectedVendors = rejectedVendors;
            this.rejectedCategories = rejectedCategories;
        }
    }

    public class GranularStatus
    {
        public string? vendorConsent;
        public string? vendorLegInt;
        public string? purposeConsent;
        public string? purposeLegInt;
        public bool? previousOptInAll;
        public bool? defaultConsent;

        public GranularStatus(string? vendorConsent, string? vendorLegInt, string? purposeConsent, string? purposeLegInt, bool? previousOptInAll, bool? defaultConsent)
        {
            this.vendorConsent = vendorConsent;
            this.vendorLegInt = vendorLegInt;
            this.purposeConsent = purposeConsent;
            this.purposeLegInt = purposeLegInt;
            this.previousOptInAll = previousOptInAll;
            this.defaultConsent = defaultConsent;
        }
    }
}