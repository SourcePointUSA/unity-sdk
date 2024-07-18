using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsentManagementProvider.Json
{
    internal class UsnatConsentWrapper
    {
#nullable enable
        public string? uuid;
        public Dictionary<string, object>? GPPData;
#nullable disable
        public bool applies;
    }

    internal class UserConsentsWrapper
    {
        public List<ConsentableWrapper> vendors;
        public List<ConsentableWrapper> categories;
    }

    internal class ConsentableWrapper
    {
#if UNITY_IOS && !UNITY_EDITOR_OSX
        [JsonProperty("_id")]
#endif
        public string id;
        public bool consented;
    }

    internal class ConsentStringWrapper
    {
        public int sectionId;
        public string sectionName;
        public string consentString;
    }
}