using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class CcpaConsent
    {
        [JsonInclude] public string uuid;
        [JsonInclude] public string status;
        [JsonInclude] public string uspstring;
        [JsonInclude] public List<string> rejectedVendors;
        [JsonInclude] public List<string> rejectedCategories;

        public CcpaConsent(string uuid, string status, string uspstring, List<string> rejectedVendors, List<string> rejectedCategories)
        {
            this.uuid = uuid;
            this.status = status;
            this.uspstring = uspstring;
            this.rejectedVendors = rejectedVendors;
            this.rejectedCategories = rejectedCategories;
        }
        
        public CcpaConsent(string uuid, string status, string uspstring, string[] rejectedVendors, string[] rejectedCategories)
        {
            this.uuid = uuid;
            this.status = status;
            this.uspstring = uspstring;
            if (this.rejectedCategories == null)
            {
                this.rejectedCategories = new List<string>();
            }
            this.rejectedCategories.AddRange(rejectedCategories);
            if (this.rejectedVendors == null)
            {
                this.rejectedVendors = new List<string>();
            }
            this.rejectedVendors.AddRange(rejectedVendors);
        }
        
    }
}