using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GdprGetMessagesConsent // : GdprConsent
{
    [JsonInclude] public bool? consentedToAl;
    [JsonInclude] public bool? rejectedAny;
    [JsonInclude] public object? childPmId;
        
    [JsonInclude] public string uuid;
    [JsonInclude] public string euconsent;
    [JsonInclude] public Dictionary<string, object> TCData;
    [JsonInclude] public Dictionary<string, SpGetMessagesVendorGrant> grants;
        
    [JsonInclude] public string addtlConsent;
    [JsonInclude] public string dateCreated;
    [JsonInclude] public bool hasConsentData;
}