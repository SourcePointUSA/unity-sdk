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
        public SPGCMData googleConsentMode;

        public string ToFullString()
        {
            StringBuilder sb = new StringBuilder();

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

            sb.AppendLine($"adStorage: {googleConsentMode.adStorage.ToString()}, analyticsStorage: {googleConsentMode.analyticsStorage.ToString()}, adUserData: {googleConsentMode.adUserData.ToString()}, adPersonalization: {googleConsentMode.adPersonalization.ToString()}");

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

        public SPGCMData(Status? adStorage, Status? analyticsStorage, 
            Status? adUserData, Status? adPersonalization)
        {
            this.adStorage = adStorage;
            this.analyticsStorage = analyticsStorage;
            this.adUserData = adUserData;
            this.adPersonalization = adPersonalization;
        }
    }

    public class ConsentStatus
    {
        public bool? rejectedAny;
        public bool? rejectedLI;
        public bool? consentedAll;
        public bool? consentedToAny;
        public bool? vendorListAdditions;
        public bool? legalBasisChanges;
        public GranularStatus? granularStatus;
        public bool hasConsentData;
        public object rejectedVendors;
        public object rejectedCategories;

        public ConsentStatus(bool? rejectedAny, bool? rejectedLI, bool? consentedAll, 
            bool? consentedToAny, bool? vendorListAdditions, bool? legalBasisChanges, 
            bool? hasConsentData, GranularStatus? granularStatus, object rejectedVendors, object rejectedCategories)
        {
            this.rejectedAny = rejectedAny;
            this.rejectedLI = rejectedLI;
            this.consentedAll = consentedAll;
            this.consentedToAny = consentedToAny;
            this.vendorListAdditions = vendorListAdditions;
            this.legalBasisChanges = legalBasisChanges;
            this.hasConsentData = hasConsentData ?? false;
            this.granularStatus = granularStatus;
            this.rejectedVendors = rejectedVendors;
            this.rejectedCategories = rejectedCategories;
        }
    }

    public class GranularStatus
    {
        public string? vendorConsent;
        public string? vendorLegInt;
        public string? purposeConsent;
        public string? purposeLegInt;
        public bool? previousOptInAll;
        public bool? defaultConsent;
        public GranularStatus(string? vendorConsent, string? vendorLegInt, string? purposeConsent, string? purposeLegInt, bool? previousOptInAll, bool? defaultConsent)
        {
            this.vendorConsent = vendorConsent;
            this.vendorLegInt = vendorLegInt;
            this.purposeConsent = purposeConsent;
            this.purposeLegInt = purposeLegInt;
            this.previousOptInAll = previousOptInAll;
            this.defaultConsent = defaultConsent;
        }
    }
}