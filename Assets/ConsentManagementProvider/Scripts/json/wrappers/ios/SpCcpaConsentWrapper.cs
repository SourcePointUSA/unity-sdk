using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpCcpaConsentWrapper
    {
        [JsonInclude] public object applies;
        [JsonInclude] public CcpaConsentWrapper consents;
    }
}