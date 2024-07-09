using System.Collections.Generic;

namespace ConsentManagementProvider.Json
{
    internal class GranularStatusWrapper
    {
#nullable enable
        public string? vendorConsent;
        public string? vendorLegInt;
        public string? purposeConsent;
        public string? purposeLegInt;
        public bool? previousOptInAll;
        public bool? defaultConsent;
        public bool? sellStatus;
        public bool? shareStatus;
        public bool? sensitiveDataStatus;
        public bool? gpcStatus;
#nullable disable
    }
}