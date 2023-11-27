using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class GranularStatusWrapper
    {
        [JsonInclude] public string? vendorConsent;
        [JsonInclude] public string? vendorLegInt;
        [JsonInclude] public string? purposeConsent;
        [JsonInclude] public string? purposeLegInt;
        [JsonInclude] public bool? previousOptInAll;
        [JsonInclude] public bool? defaultConsent;
    }
}