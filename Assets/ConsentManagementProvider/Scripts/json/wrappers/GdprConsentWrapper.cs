using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class GdprConsentWrapper
    {
        [JsonInclude] public string uuid;
        [JsonInclude] public string euconsent;
        [JsonInclude] public Dictionary<string, object> TCData;
        [JsonInclude] public Dictionary<string, SpVendorGrantWrapper> grants;
        [JsonInclude] public List<string> acceptedCategories;
        [JsonInclude] public bool applies;
        [JsonInclude] public string webConsentPayload;
        [JsonInclude] public object consentStatus;
    }
}