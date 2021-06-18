using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpCcpaConsentWrapper
    {
        [JsonInclude] public object applies;
        [JsonInclude] public CcpaConsentWrapper consents;
    }
}