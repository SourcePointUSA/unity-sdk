using System;

namespace ConsentManagementProviderLib.Json
{
    internal class CcpaConsentWrapper
    {
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