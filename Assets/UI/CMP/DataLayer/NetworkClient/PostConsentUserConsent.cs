using System.Collections.Generic;
using System.Text.Json.Serialization;

public class PostConsentUserConsent
{
    // GDPR
    [JsonInclude] public Dictionary<string, object> TCData;
    [JsonInclude] public Dictionary<string, SpGetMessagesVendorGrant> grants;

    [JsonInclude] public string[] specialFeatures;
    [JsonInclude] public string[] legIntCategories;
    [JsonInclude] public string[] acceptedVendors;
    [JsonInclude] public string[] acceptedCategories;
    
    [JsonInclude] public string euconsent;
    [JsonInclude] public string addtlConsent;
    [JsonInclude] public string dateCreated;

    [JsonInclude] public bool consentedToAll;
    
    //CCPA 
    [JsonInclude] public string uspstring;
    [JsonInclude] public string status;
    [JsonInclude] public string[] rejectedVendors;
    [JsonInclude] public string[] rejectedCategories;
    [JsonInclude] public bool signedLspa;
    [JsonInclude] public bool rejectedAll;

    public string uuid;
}