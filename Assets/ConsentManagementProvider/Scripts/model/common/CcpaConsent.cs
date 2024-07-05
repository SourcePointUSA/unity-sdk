using System;
using System.Text;
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
        public Dictionary<string, object>? GPPData;
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
                        ConsentStatus? consentStatus,
                        Dictionary<string, object>? GPPData
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
            this.GPPData = GPPData;
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
						ConsentStatus consentStatus,
                        Dictionary<string, object>? GPPData
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
            this.GPPData = GPPData;
        }
        
           public string ToFullString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"CCPA");

            sb.AppendLine($"UUID: {uuid}");
            sb.AppendLine($"Applies: {applies}");
            sb.AppendLine($"Status: {status}");
            sb.AppendLine($"Uspstring: {uspstring}");
            sb.AppendLine($"ChildPmId: {childPmId}");
            if (signedLspa != null)
                sb.AppendLine($"SignedLspa: {signedLspa}");
            sb.AppendLine($"WebConsentPayload: {webConsentPayload}");
            
            if(rejectedVendors != null)
            {
                sb.AppendLine("Rejected Vendors:");
                foreach (var vendor in rejectedVendors)
                    sb.AppendLine($"    {vendor}");
            }

            if(rejectedCategories != null)
            {
                sb.AppendLine("Rejected Categories:");
                foreach (var category in rejectedCategories)
                    sb.AppendLine($"    {category}");
            }

            if (consentStatus != null)
                sb = consentStatus.ToFullString(sb);

            if(GPPData != null)
            {
                sb.AppendLine("GPPData:");
                foreach (var kvp in GPPData)
                    sb.AppendLine($"    {kvp.Key}: {kvp.Value.ToString()}");
            }

            return sb.ToString();
        }
    }
}