using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpGdprConsentWrapperAndroid
    {
        [JsonInclude] public string uuid;
        [JsonInclude] public string euconsent;
        [JsonInclude] public Dictionary<string, object> tcData;
        [JsonInclude] public Dictionary<string, Dictionary<string, object>> grants;
    }
}