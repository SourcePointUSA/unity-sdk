using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class CustomGdprAndroid : GdprConsent
    {
        [JsonInclude] public string addtlConsent;
        [JsonInclude] public string childPmId;
        [JsonInclude] public bool consentedToAll;
        [JsonInclude] public string dateCreated;
        [JsonInclude] public bool hasConsentData;
        [JsonInclude] public bool rejectedAny;
    }
}