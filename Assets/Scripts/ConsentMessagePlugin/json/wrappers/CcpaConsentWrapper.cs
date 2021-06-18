using System;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class CcpaConsentWrapper
    {
        [JsonInclude] public string status;
        [JsonInclude] public string uspstring;
        [JsonInclude] public string[] rejectedVendors;
        [JsonInclude] public string[] rejectedCategories;
    }
}