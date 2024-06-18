using System;
using System.Collections.Generic;
using ConsentManagementProviderLib.Android;
using ConsentManagementProviderLib.iOS;
using ConsentManagementProviderLib.UnityEditor;
using ConsentManagementProviderLib.Observer;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class CMP
    {
        private static GameObject mainThreadBroadcastEventsExecutor;
        private static IMobile instance;
        public static IMobile Instance
        {
            get
            {
                if (instance == null)
#if UNITY_ANDROID
                    instance = new ConsentWrapperAndroid();
#elif UNITY_IOS && !UNITY_EDITOR_OSX
                    instance = new ConsentWrapperIOS();
#else
                    instance = new ConsentWrapperUnityEditor();
#endif
                return instance;
            }
        }

        public static bool useGDPR = false;
        public static bool useCCPA = false;
        public static bool useUSNAT = false;
        
        public static void Initialize(
            List<SpCampaign> spCampaigns, 
            int accountId,
            int propertyId,
            string propertyName,
            MESSAGE_LANGUAGE language,  
            string gdprPmId, 
            string ccpaPmId,
            string usnatPmId,
            CAMPAIGN_ENV campaignsEnvironment,
            long messageTimeoutInSeconds = 3,
            bool? transitionCCPAAuth = null,
            bool? supportLegacyUSPString = null)
        {
            if(!IsSpCampaignsValid(spCampaigns))
            { 
                return;
            }

            foreach (SpCampaign sp in spCampaigns)
            {
                switch (sp.CampaignType)
                {
                    case CAMPAIGN_TYPE.GDPR: useGDPR = true; break;
                    case CAMPAIGN_TYPE.CCPA: useCCPA = true; break;
                    case CAMPAIGN_TYPE.USNAT: useUSNAT = true; break;
                }
            }

            propertyName = propertyName.Trim();
            gdprPmId = gdprPmId.Trim();
            ccpaPmId = ccpaPmId.Trim();
            usnatPmId = usnatPmId.Trim();

            CreateBroadcastExecutorGO();

            Instance.InitializeLib(
                accountId: accountId, 
                propertyId: propertyId, 
                propertyName: propertyName, 
                gdpr: useGDPR,
                ccpa: useCCPA,
                usnat: useUSNAT,
                language: language, 
                gdprPmId: gdprPmId, 
                ccpaPmId: ccpaPmId,
                usnatPmId: usnatPmId,
                spCampaigns: spCampaigns,
                campaignsEnvironment: campaignsEnvironment,
                messageTimeoutInSeconds: messageTimeoutInSeconds,
                transitionCCPAAuth: transitionCCPAAuth,
                supportLegacyUSPString: supportLegacyUSPString);
        }

        public static void LoadMessage(string authId = null) => Instance.LoadMessage(authId: authId);

        public static void ClearAllData(string authId = null) => Instance.ClearAllData();

        public static void LoadPrivacyManager(
            CAMPAIGN_TYPE campaignType, 
            string pmId, 
            PRIVACY_MANAGER_TAB tab) => Instance.LoadPrivacyManager(
                                            campaignType: campaignType, 
                                            pmId: pmId, 
                                            tab: tab);

        public static void CustomConsentGDPR(
            string[] vendors, 
            string[] categories, 
            string[] legIntCategories, 
            Action<GdprConsent> onSuccessDelegate) => Instance.CustomConsentGDPR(
                                                        vendors: vendors,
                                                        categories: categories,
                                                        legIntCategories: legIntCategories,
                                                        onSuccessDelegate: onSuccessDelegate);

        public static void DeleteCustomConsentGDPR(
            string[] vendors, 
            string[] categories, 
            string[] legIntCategories, 
            Action<GdprConsent> onSuccessDelegate) => Instance.DeleteCustomConsentGDPR(
                                                        vendors: vendors,
                                                        categories: categories,
                                                        legIntCategories: legIntCategories,
                                                        onSuccessDelegate: onSuccessDelegate);

        public static SpConsents GetSpConsents() => Instance.GetSpConsents();

        public static GdprConsent GetCustomConsent() => Instance.GetCustomConsent();

        public static void Dispose() => Instance.Dispose();
        
        #region private methods
        private static void CreateBroadcastExecutorGO()
        {
            if (mainThreadBroadcastEventsExecutor != null) return;
            mainThreadBroadcastEventsExecutor = new GameObject();
            mainThreadBroadcastEventsExecutor.AddComponent<BroadcastEventsExecutor>();
        }

        private static bool IsSpCampaignsValid(List<SpCampaign> spCampaigns)
        {
            //check if there more than 0 campaign
            if (spCampaigns.Count == 0)
            {
                Debug.LogError("You should add at least one SpCampaign to use CMP! Aborting...");
                return false;
            }
            
            return true;
        }
        #endregion
    }
}