using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpVendorGrantWrapper
    {
        [JsonInclude]
        public object vendorGrant;
        [JsonInclude]
        public Dictionary<string, object> purposeGrants;
    }
}