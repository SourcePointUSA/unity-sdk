using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GdprGetMessagesConsent // : GdprConsent
{
#nullable enable
    [JsonInclude] public bool? consentedToAll;
    [JsonInclude] public bool? rejectedAny;
    [JsonInclude] public object? childPmId;
#nullable disable

    [JsonInclude] public string uuid;
    [JsonInclude] public string euconsent;
    [JsonInclude] public Dictionary<string, object> TCData;
    [JsonInclude] public Dictionary<string, SpGetMessagesVendorGrant> grants;
        
    [JsonInclude] public string addtlConsent;
    [JsonInclude] public string dateCreated;
    [JsonInclude] public bool hasConsentData;
}