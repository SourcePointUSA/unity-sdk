using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class ConsentStatusWrapper
    {
#nullable enable
        public bool? rejectedAny;
        public bool? rejectedLI;
        public bool? consentedAll;
        public bool? consentedToAny;
        public bool? vendorListAdditions;
        public bool? legalBasisChanges;
        public GranularStatusWrapper? granularStatus;
#nullable disable
        public bool hasConsentData;
        public object rejectedVendors;
        public object rejectedCategories;
    }
}