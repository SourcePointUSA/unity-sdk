using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class ConsentStatusWrapper
    {
#nullable enable
        public bool? rejectedAny;
        public bool? rejectedLI;
        public bool? consentedAll;
        public bool? consentedToAll;
        public bool? consentedToAny;
        public bool? rejectedAll;
        public bool? vendorListAdditions;
        public bool? legalBasisChanges;
        public GranularStatusWrapper? granularStatus;
        public object? rejectedVendors;
        public object? rejectedCategories;
#nullable disable
        public bool hasConsentData;
    }
}