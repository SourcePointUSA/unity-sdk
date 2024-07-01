using System;
using Newtonsoft.Json;

namespace ConsentManagementProviderLib.Json
{
    internal class SpCcpaConsentWrapperAndroid: CcpaConsentWrapper
    {
        [JsonProperty("apply")]
        public bool applies;
    }
}