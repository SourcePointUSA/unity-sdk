using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class UsnatConsentWrapper
    {
#nullable enable
        public string? uuid;
        public bool applies;
        public string? webConsentPayload;
		public ConsentStatusWrapper? consentStatus;
#nullable disable
        public List<ConsentStringWrapper> consentStrings;
        public string[] categories;
    }

    internal class ConsentStringWrapper
    {
        public string consentString;
        public string sectionId;
        public string sectionName;
    }
}