using Newtonsoft.Json;
using System.Collections.Generic;

namespace ConsentManagementProvider.Json
{
    internal class SpUsnatConsentWrapperAndroid: UsnatConsentWrapper
    {
        public StatusWrapperAndroid statuses;
        public string consentStrings;
        public string vendors;
        public string categories;
    }

    internal class StatusWrapperAndroid
    {
#nullable enable
        public bool? hasConsentData;
        public bool? rejectedAny;
        public bool? consentedToAll;
        public bool? consentedToAny;
        public bool? sellStatus;
        public bool? shareStatus;
        public bool? sensitiveDataStatus;
        public bool? gpcStatus;
#nullable disable
    }
}
