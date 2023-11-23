using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpCcpaConsentWrapper
    {
        [JsonInclude] public bool applies;
        [JsonInclude] public CcpaConsentWrapper consents;
    }
}