using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsentManagementProviderLib.Json
{
    internal class UsnatConsentWrapper
    {
#nullable enable
        public string? uuid;
#nullable disable
		public ConsentStatusWrapper consentStatus;
        public bool applies;
        public List<ConsentStringWrapper> consentStrings;
        public UserConsentsWrapper userConsents;

        [JsonIgnore]
        public List<ConsentableWrapper> vendors { get => userConsents.vendors; }
        [JsonIgnore]
        public List<ConsentableWrapper> categories { get => userConsents.categories; }
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