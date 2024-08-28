using System;

namespace ConsentManagementProvider
{
    public static class CMPTestUtils
    {
        static bool delegateCalled = false;
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