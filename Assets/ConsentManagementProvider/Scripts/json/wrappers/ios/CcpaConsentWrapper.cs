using System;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class CcpaConsentWrapper
    {
        [JsonInclude] public string uuid;
        [JsonInclude] public string status;
        [JsonInclude] public string uspstring;
        [JsonInclude] public string[] rejectedVendors;
        [JsonInclude] public string[] rejectedCategories;
        [JsonInclude] public string childPmId;
        [JsonInclude] public bool applies;
        [JsonInclude] public bool signedLspa;
        [JsonInclude] public string webConsentPayload;
        [JsonInclude] public ConsentStatusWrapper consentStatus;
    }
}