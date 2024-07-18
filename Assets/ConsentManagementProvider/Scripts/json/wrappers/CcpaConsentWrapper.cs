using System.Collections.Generic;

namespace ConsentManagementProvider.Json
{
    internal class CcpaConsentWrapper
    {
#nullable enable
        public Dictionary<string, object>? GPPData;
#nullable disable
        public string uuid;
        public string status;
        public string uspstring;
        public string[] rejectedVendors;
        public string[] rejectedCategories;
        public string childPmId;
        public bool signedLspa;
        public string webConsentPayload;
        public ConsentStatusWrapper consentStatus;
    }
}