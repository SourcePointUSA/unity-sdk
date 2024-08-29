using System;
using System.Collections.Generic;

namespace ConsentManagementProvider
{
    public static class CMPTestUtils
    {
        static bool delegateCalled = false;

        public static void InitializeWithLanguage(
            string languageKey)
        {
            MESSAGE_LANGUAGE language = (MESSAGE_LANGUAGE)Int32.Parse(languageKey);

            List<CAMPAIGN_TYPE> campaignTypes = new ();
            List<SpCampaign> spCampaigns = new List<SpCampaign>();
            List<TargetingParam> gdprParams = new List<TargetingParam> { new TargetingParam("location", "EU") };
            SpCampaign gdpr = new SpCampaign(CAMPAIGN_TYPE.GDPR, gdprParams);
            spCampaigns.Add(gdpr);
            campaignTypes.Add(CAMPAIGN_TYPE.GDPR);

            List<TargetingParam> ccpaParams = new List<TargetingParam> { new TargetingParam("location", "US") };
            SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.CCPA, ccpaParams);
            spCampaigns.Add(ccpa);
            campaignTypes.Add(CAMPAIGN_TYPE.CCPA);

            List<TargetingParam> usnatParams = new List<TargetingParam> { new TargetingParam("location", "US") };
            SpCampaign usnat = new SpCampaign(CAMPAIGN_TYPE.USNAT, usnatParams, transitionCCPAAuth: false, supportLegacyUSPString: false);
            spCampaigns.Add(usnat);
            campaignTypes.Add(CAMPAIGN_TYPE.USNAT);

            CMP.Instance.Initialize(
                spCampaigns: spCampaigns,
                accountId: 22,
                propertyId: 16893,
                propertyName: "mobile.multicampaign.demo",
                language: language,
                campaignsEnvironment: CAMPAIGN_ENV.PUBLIC,
                messageTimeoutInSeconds: 30
        );
        }

        public static void LoadPrivacyManager(
            string campaignType, 
            string pmId)
        {
            CAMPAIGN_TYPE type = (CAMPAIGN_TYPE)Int32.Parse(campaignType);
            CMP.ConcreteInstance.LoadPrivacyManager(
                campaignType: type,
                pmId: pmId);
        }

        public static void CustomConsentGDPR(string arg)
        {
            string[] vendors = { "5fbe6f050d88c7d28d765d47", "5ff4d000a228633ac048be41" };
            string[] categories = { "60657acc9c97c400122f21f3", "608bad95d08d3112188e0e36", "608bad95d08d3112188e0e2f" };
            string[] legIntCategories = { };
            delegateCalled = false;
            CMP.ConcreteInstance.CustomConsentGDPR(vendors, categories, legIntCategories, onSuccessDelegate);
        }

        public static void DeleteCustomConsentGDPR(string arg)
        {
            string[] vendors = { "5fbe6f050d88c7d28d765d47", "5ff4d000a228633ac048be41" };
            string[] categories = { "60657acc9c97c400122f21f3", "608bad95d08d3112188e0e36", "608bad95d08d3112188e0e2f" };
            string[] legIntCategories = { };
            delegateCalled = false;
            CMP.ConcreteInstance.DeleteCustomConsentGDPR(vendors, categories, legIntCategories, onSuccessDelegate);
        }

        private static void onSuccessDelegate(GdprConsent customConsent)
        {
            delegateCalled = true;
        }
    }
}