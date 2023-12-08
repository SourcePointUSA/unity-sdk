using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class GranularStatusWrapper
    {
        public string? vendorConsent;
        public string? vendorLegInt;
        public string? purposeConsent;
        public string? purposeLegInt;
        public bool? previousOptInAll;
        public bool? defaultConsent;
    }
}