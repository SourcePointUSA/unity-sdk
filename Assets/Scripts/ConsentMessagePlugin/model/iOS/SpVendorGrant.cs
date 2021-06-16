using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpVendorGrant
    {
        [JsonInclude]
        public bool vendorGrant;
        [JsonInclude]
        public Dictionary<string, bool> purposeGrants;
    }
}