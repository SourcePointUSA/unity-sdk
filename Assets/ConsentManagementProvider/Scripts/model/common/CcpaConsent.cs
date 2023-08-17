using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class CcpaConsent
    {
        [JsonInclude] public bool applies;
        [JsonInclude] public string uuid;
        [JsonInclude] public string webConsentPayload;
        [JsonInclude] public string status;
        [JsonInclude] public string uspstring;
        [JsonInclude] public List<string> rejectedVendors;
        [JsonInclude] public List<string> rejectedCategories;
        [JsonInclude] public ConsentStatus consentStatus;

        public CcpaConsent(bool applies, string uuid, string webConsentPayload, string status, string uspstring, List<string> rejectedVendors, List<string> rejectedCategories, ConsentStatus consentStatus)
        {
            this.applies = applies;
            this.uuid = uuid;
            this.webConsentPayload = webConsentPayload;
            this.status = status;
            this.uspstring = uspstring;
            this.rejectedVendors = rejectedVendors;
            this.rejectedCategories = rejectedCategories;
            this.consentStatus = consentStatus;
        }
        
        public CcpaConsent(bool applies, string uuid, string webConsentPayload, string status, string uspstring, string[] rejectedVendors, string[] rejectedCategories, ConsentStatus consentStatus)
        {
            this.applies = applies;
            this.uuid = uuid;
            this.webConsentPayload = webConsentPayload;
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
            this.consentStatus = consentStatus;
        }
        
    }
}