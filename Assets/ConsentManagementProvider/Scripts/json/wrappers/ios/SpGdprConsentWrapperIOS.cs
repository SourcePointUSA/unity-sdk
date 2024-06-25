using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class SpGdprConsentWrapperIOS: GdprConsentWrapper
    {
        public bool applies;
        public ConsentStatusWrapper consentStatus;
#nullable enable
        public GCMDataWrapper? gcmStatus;
#nullable disable
    }
}