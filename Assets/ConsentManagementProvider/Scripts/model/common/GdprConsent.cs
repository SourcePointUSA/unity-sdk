using System.Collections.Generic;
using System.Text;

namespace ConsentManagementProviderLib
{
    public class GdprConsent
    {
        public bool applies;
        public string uuid;
        public string webConsentPayload;
        public string euconsent;
        public Dictionary<string, object> TCData;
        public Dictionary<string, SpVendorGrant> grants;
		public List<string> acceptedCategories;
        public ConsentStatus consentStatus;
#nullable enable
        public SPGCMData? googleConsentMode;
#nullable disable

        public string ToFullString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"GDPR");

            sb.AppendLine($"UUID: {uuid}");
            sb.AppendLine($"EUConsent: {euconsent}");
            sb.AppendLine($"Applies: {applies}");
            sb.AppendLine($"WebConsentPayload: {webConsentPayload}");

            if(TCData != null)
            {
                sb.AppendLine("TCData:");
//                 foreach (var kvp in TCData)
//                     sb.AppendLine($"    {kvp.Key}: {kvp.Value}");
            }

            if(grants != null)
            {
                sb.AppendLine("Grants:");
                foreach (var grant in grants)
                {
                    sb.AppendLine($"    Vendor: {grant.Key}");
                    sb.AppendLine($"    VendorGrant: {grant.Value.vendorGrant}");

                    if(grant.Value.purposeGrants != null)
                    {
                        sb.AppendLine("    Purpose Grants:");
                        foreach (var purposeGrant in grant.Value.purposeGrants)
                            sb.AppendLine($"        {purposeGrant.Key}: {purposeGrant.Value}");
                    }
                }
            }

            if(acceptedCategories != null)
            {
                sb.AppendLine("Accepted Categories:");
                foreach (var category in acceptedCategories)
                    sb.AppendLine($"    {category}");
            }

            if(consentStatus != null)
                sb = consentStatus.ToFullString(sb);

            if(googleConsentMode != null)
                sb.AppendLine($"adStorage: {googleConsentMode.adStorage}, analyticsStorage: {googleConsentMode.analyticsStorage}, adUserData: {googleConsentMode.adUserData}, adPersonalization: {googleConsentMode.adPersonalization}");

            return sb.ToString();
        }
    }

    public class SPGCMData
    {
        public enum Status
        {
            granted,
            denied
        }
        public Status? adStorage, analyticsStorage, adUserData, adPersonalization;

        public SPGCMData(
            Status? adStorage, 
            Status? analyticsStorage, 
            Status? adUserData, 
            Status? adPersonalization)
        {
            this.adStorage = adStorage;
            this.analyticsStorage = analyticsStorage;
            this.adUserData = adUserData;
            this.adPersonalization = adPersonalization;
        }
    }

    public class ConsentStatus
    {
#nullable enable
        public bool? rejectedAny;
        public bool? rejectedLI;
        public bool? consentedAll;
        public bool? consentedToAll;
        public bool? consentedToAny;
        public bool? rejectedAll;
        public bool? vendorListAdditions;
        public bool? legalBasisChanges;
        public GranularStatus? granularStatus;
#nullable disable
        public bool hasConsentData;
        public object rejectedVendors;
        public object rejectedCategories;

        public StringBuilder ToFullString(StringBuilder sb)
        {
            sb.AppendLine("ConsentStatus:");
            if(rejectedAny != null)
                sb.AppendLine($"    rejectedAny: {rejectedAny}");
            if(rejectedLI != null)
                sb.AppendLine($"    rejectedLI: {rejectedLI}");
            if(consentedAll != null)
                sb.AppendLine($"    consentedAll: {consentedAll}");
            if(consentedToAny != null)
                sb.AppendLine($"    consentedToAny: {consentedToAny}");
            if(vendorListAdditions != null)
                sb.AppendLine($"    vendorListAdditions: {vendorListAdditions}");
            if(legalBasisChanges != null)
                sb.AppendLine($"    legalBasisChanges: {legalBasisChanges}");
            sb.AppendLine($"    hasConsentData: {hasConsentData}");
            
            if(granularStatus != null)
                sb = granularStatus.ToFullString(sb);

            return sb;
        }
    }

    public class GranularStatus
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

        public GranularStatus(
#nullable enable
            string? vendorConsent, 
            string? vendorLegInt, 
            string? purposeConsent, 
            string? purposeLegInt, 
            bool? previousOptInAll, 
            bool? defaultConsent,
            bool? sellStatus,
            bool? shareStatus,
            bool? sensitiveDataStatus,
            bool? gpcStatus)
#nullable disable
        {
            this.vendorConsent = vendorConsent;
            this.vendorLegInt = vendorLegInt;
            this.purposeConsent = purposeConsent;
            this.purposeLegInt = purposeLegInt;
            this.previousOptInAll = previousOptInAll;
            this.defaultConsent = defaultConsent;
            this.sellStatus = sellStatus;
            this.shareStatus = shareStatus;
            this.sensitiveDataStatus = sensitiveDataStatus;
            this.gpcStatus = gpcStatus;
        }

        public StringBuilder ToFullString(StringBuilder sb)
        {
            sb.AppendLine("    GranularStatus:");
            if(vendorConsent != null)
                sb.AppendLine($"        vendorConsent: {vendorConsent}");
            if(vendorLegInt != null)
                sb.AppendLine($"        vendorLegInt: {vendorLegInt}");
            if(purposeConsent != null)
                sb.AppendLine($"        purposeConsent: {purposeConsent}");
            if(purposeLegInt != null)
                sb.AppendLine($"        purposeLegInt: {purposeLegInt}");
            if(previousOptInAll != null)
                sb.AppendLine($"        previousOptInAll: {previousOptInAll}");
            if(defaultConsent != null)
                sb.AppendLine($"        defaultConsent: {defaultConsent}");
            if(sellStatus != null)
                sb.AppendLine($"        sellStatus: {sellStatus}");
            if(shareStatus != null)
                sb.AppendLine($"        shareStatus: {shareStatus}");
            if(sensitiveDataStatus != null)
                sb.AppendLine($"        sensitiveDataStatus: {sensitiveDataStatus}");
            if(gpcStatus != null)
                sb.AppendLine($"        gpcStatus: {gpcStatus}");
            
            return sb;
        }
    }
}