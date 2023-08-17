using System;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class CcpaConsentWrapper
    {
        [JsonInclude] public bool applies;
        [JsonInclude] public string uuid;
        [JsonInclude] public string webConsentPayload;
        [JsonInclude] public string status;
        [JsonInclude] public string uspstring;
        [JsonInclude] public string[] rejectedVendors;
        [JsonInclude] public string[] rejectedCategories;
        [JsonInclude] public object consentStatus;
    }
}