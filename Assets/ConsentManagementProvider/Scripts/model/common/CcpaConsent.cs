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
        [JsonInclude] public string childPmId;
        [JsonInclude] public bool applies;
        [JsonInclude] public bool? signedLspa;
        [JsonInclude] public string webConsentPayload;
		[JsonInclude] public ConsentStatus? consentStatus;

        public CcpaConsent(
                        string uuid,
                        string status,
                        string uspstring,
                        List<string> rejectedVendors,
                        List<string> rejectedCategories,
                        string childPmId,
                        bool applies,
                        bool? signedLspa,
                        string webConsentPayload,
						ConsentStatus? consentStatus
        ) {
            this.uuid = uuid;
            this.webConsentPayload = webConsentPayload;
            this.status = status;
            this.uspstring = uspstring;
            this.rejectedVendors = rejectedVendors;
            this.rejectedCategories = rejectedCategories;
            this.childPmId = childPmId;
            this.applies = applies;
            this.signedLspa = signedLspa;
            this.webConsentPayload = webConsentPayload;
			this.consentStatus = consentStatus;
        }
        
        public CcpaConsent(
                        string uuid,
                        string status,
                        string uspstring,
                        string[] rejectedVendors,
                        string[] rejectedCategories,
                        string childPmId,
                        bool applies,
                        bool? signedLspa,
                        string webConsentPayload,
						ConsentStatus consentStatus
        ) {
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
            this.childPmId = childPmId;
            this.applies = applies;
            this.signedLspa = signedLspa;
            this.webConsentPayload = webConsentPayload;
			this.consentStatus = consentStatus;
        }
        
    }
}