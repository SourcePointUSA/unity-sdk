using System.Collections.Generic;

namespace ConsentManagementProvider.Json
{
    internal class SpGdprConsentWrapperIOS: GdprConsentWrapper
    {
        public bool applies;
#nullable enable
        public GCMDataWrapper? gcmStatus;
#nullable disable
    }
}