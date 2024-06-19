using System;
using System.Collections.Generic;

namespace ConsentManagementProviderLib
{
    public interface ISpSdk
    {
        void InitializeLib(
            int accountId, 
            int propertyId, 
            string propertyName, 
            bool gdpr,
            bool ccpa,
            bool usnat,
            MESSAGE_LANGUAGE language, 
            string gdprPmId, 
            string ccpaPmId,
            string usnatPmId,
            List<SpCampaign> spCampaigns,
            CAMPAIGN_ENV campaignsEnvironment, 
            long messageTimeoutInSeconds = 3,
            bool? transitionCCPAAuth = null,
            bool? supportLegacyUSPString = null);
        void LoadMessage(string authId = null);
        void LoadPrivacyManager(CAMPAIGN_TYPE campaignType, string pmId, PRIVACY_MANAGER_TAB tab);
        void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate);
        void DeleteCustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate);
        SpConsents GetSpConsents();
        GdprConsent GetCustomConsent();
        void ClearAllData();
        void Dispose();
    }
}