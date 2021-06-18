using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpGdprConsentWrapper
    {
        [JsonInclude]
        public string euconsent;
        [JsonInclude]
        public Dictionary<string, object> TCData;
        [JsonInclude]
        public Dictionary<string, SpVendorGrantWrapper> grants;
    }
}