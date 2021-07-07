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
    }
}