using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CcpaGetMessagesConsent
{
    [JsonInclude] public string dateCreated;
    [JsonInclude] public bool newUser;
    [JsonInclude] public List<string> rejectedCategories;
    [JsonInclude] public List<string> rejectedVendors;
    [JsonInclude] public bool rejectedAll;
    [JsonInclude] public string status;
    [JsonInclude] public bool signedLspa;
    [JsonInclude] public string uspstring;
}