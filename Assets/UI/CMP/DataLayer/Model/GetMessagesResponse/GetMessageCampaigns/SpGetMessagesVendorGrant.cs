using System.Collections.Generic;
using System.Text.Json.Serialization;

public class SpGetMessagesVendorGrant
{
    [JsonInclude] public bool vendorGrant;
    [JsonInclude] public Dictionary<string, bool> purposeGrants;
}