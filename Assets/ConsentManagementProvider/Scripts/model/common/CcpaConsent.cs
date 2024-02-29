using System;
using System.Collections.Generic;

namespace ConsentManagementProviderLib
{
    public class CcpaConsent
    {
        public string uuid;
        public string status;
        public string uspstring;
        public List<string> rejectedVendors;
        public List<string> rejectedCategories;
        public string childPmId;
        public bool applies;
#nullable enable
        public bool? signedLspa;
        public string webConsentPayload;
		public ConsentStatus? consentStatus;
#nullable disable

        public CcpaConsent(
#nullable enable
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
#nullable disable
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
#nullable enable
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
#nullable disable
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