using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class GdprConsentWrapper
    {
        public string uuid;
        public string euconsent;
        public Dictionary<string, object> TCData;
        public Dictionary<string, SpVendorGrantWrapper> grants;
        public List<string> acceptedCategories;
        public string webConsentPayload;
        public ConsentStatusWrapper consentStatus;
    }
}