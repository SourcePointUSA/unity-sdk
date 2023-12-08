using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class ConsentStatusWrapper
    {
        public bool? rejectedAny;
        public bool? rejectedLI;
        public bool? consentedAll;
        public bool? consentedToAny;
        public bool? vendorListAdditions;
        public bool? legalBasisChanges;
        public GranularStatusWrapper? granularStatus;
        public bool hasConsentData;
        public object rejectedVendors;
        public object rejectedCategories;
    }
}