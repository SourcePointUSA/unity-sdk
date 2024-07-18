using System;
using Newtonsoft.Json;

namespace ConsentManagementProvider.Json
{
    internal class SpCcpaConsentWrapperAndroid: CcpaConsentWrapper
    {
        [JsonProperty("apply")]
        public bool applies;
    }
}