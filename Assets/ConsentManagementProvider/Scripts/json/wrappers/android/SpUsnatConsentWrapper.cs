using Newtonsoft.Json;
using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class SpUsnatConsentWrapperAndroid
    {
#nullable enable
        public string? uuid;
#nullable disable
        public StatusWrapperAndroid statuses;
        public bool applies;
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
