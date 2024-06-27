using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsentManagementProviderLib.Json
{
    internal class SpUsnatConsentWrapperIOS: UsnatConsentWrapper
    {
		public ConsentStatusWrapper consentStatus;
        public List<ConsentStringWrapper> consentStrings;
        public UserConsentsWrapper userConsents;

        [JsonIgnore]
        public List<ConsentableWrapper> vendors { get => userConsents.vendors; }
        [JsonIgnore]
        public List<ConsentableWrapper> categories { get => userConsents.categories; }
    }
}