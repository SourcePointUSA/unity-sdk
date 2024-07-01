using Newtonsoft.Json;
using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class SpGdprConsentWrapperAndroid: GdprConsentWrapper
    {
        [JsonProperty("apply")]
        public bool applies;
        [JsonProperty("googleConsentMode")]
        public GCMDataWrapper gcmStatus;
    }
}