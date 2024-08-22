using System;

namespace ConsentManagementProvider
{
    public static class CMPTestUtils
    {
        public static void LoadPrivacyManager(
            string campaignType, 
            string pmId)
        {
            CAMPAIGN_TYPE type = (CAMPAIGN_TYPE)Int32.Parse(campaignType);
            CMP.ConcreteInstance.LoadPrivacyManager(
                campaignType: type,
                pmId: pmId);
        }
    }
}