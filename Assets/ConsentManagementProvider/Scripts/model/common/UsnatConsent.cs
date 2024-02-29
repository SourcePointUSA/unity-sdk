using System;
using System.Collections.Generic;
using System.Text;

namespace ConsentManagementProviderLib
{
    public class UsnatConsent
    {
#nullable enable
        public string? uuid;
        public bool applies;
        public List<ConsentString> consentStrings;
        public List<string> categories;
        public string? webConsentPayload;
		public ConsentStatus? consentStatus;
#nullable disable

        public UsnatConsent(
#nullable enable
                        string? uuid,
                        bool applies,
                        List<ConsentString> consentStrings,
                        List<string> categories,
                        string? webConsentPayload,
						ConsentStatus? consentStatus
#nullable disable
        ) {
            this.uuid = uuid;
            this.applies = applies;
            this.consentStrings = consentStrings;
            this.categories = categories;
            this.webConsentPayload = webConsentPayload;
            this.consentStatus = consentStatus;
        }

        public UsnatConsent(
#nullable enable
                        string? uuid,
                        bool applies,
                        List<ConsentString> consentStrings,
                        string[] categories,
                        string? webConsentPayload,
						ConsentStatus? consentStatus
#nullable disable
        ) {
            this.uuid = uuid;
            this.applies = applies;
            this.consentStrings = consentStrings;
            if (this.categories == null)
            {
                this.categories = new List<string>();
            }
            this.categories.AddRange(categories);
            this.webConsentPayload = webConsentPayload;
			this.consentStatus = consentStatus;
        }
        
        public string ToFullString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"USNAT");

            if(uuid != null)
                sb.AppendLine($"UUID: {uuid}");
            sb.AppendLine($"Applies: {applies}");
            sb.AppendLine($"WebConsentPayload: {webConsentPayload}");

            sb.AppendLine($"ConsentStrings:");
            foreach (var _string in consentStrings)
            {
                sb.AppendLine($"    sectionId: {_string.sectionId}, sectionName: {_string.sectionName}, consentString: {_string.consentString}");
            }

            sb.AppendLine("Categories:");
            foreach (var _string in categories)
            {
                sb.AppendLine($"    {_string}");
            }

            if(consentStatus != null)
                sb = consentStatus.ToFullString(sb);

            return sb.ToString();
        }
    }

    public class ConsentString 
    {
        public string consentString;
        public string sectionId;
        public string sectionName;

        public ConsentString(string consentString, string sectionId, string sectionName)
        {
            this.consentString = consentString;
            this.sectionId = sectionId;
            this.sectionName = sectionName;
        }
    }
}