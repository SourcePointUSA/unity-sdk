using System.Collections.Generic;

namespace ConsentManagementProvider.Json
{
    internal class GdprConsentWrapper
    {
        public string uuid;
        public string euconsent;
        public Dictionary<string, object> tcData;
        public Dictionary<string, SpVendorGrantWrapper> grants;
        public List<string> acceptedCategories;
        public string webConsentPayload;
        public ConsentStatusWrapper consentStatus;
    }
}