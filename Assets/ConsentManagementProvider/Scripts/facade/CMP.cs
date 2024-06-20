using System;
using System.Collections.Generic;
using ConsentManagementProviderLib.Android;
using ConsentManagementProviderLib.iOS;
using ConsentManagementProviderLib.UnityEditor;
using ConsentManagementProviderLib.Observer;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public class CMP: ISpSdk
    {
        private static GameObject mainThreadBroadcastEventsExecutor;
        private static ISpSdk instance;
        internal static ISpSdk Instance
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

        private static CMP self;
        public static CMP GetInstance()
        {
            if (self == null)
                self = new CMP();
            return self;
        }

        internal static bool useGDPR = false;
        internal static bool useCCPA = false;
        internal static bool useUSNAT = false;
        
        public void Initialize(
            int accountId,
            int propertyId,
            string propertyName,
            MESSAGE_LANGUAGE language,  
            string gdprPmId, 
            string ccpaPmId,
            string usnatPmId,
            List<SpCampaign> spCampaigns, 
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

            Instance.Initialize(
                accountId: accountId, 
                propertyId: propertyId, 
                propertyName: propertyName, 
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

        public void LoadMessage(string authId = null) => Instance.LoadMessage(authId: authId);

        public void ClearAllData() => Instance.ClearAllData();

        public void LoadPrivacyManager(
            CAMPAIGN_TYPE campaignType, 
            string pmId, 
            PRIVACY_MANAGER_TAB tab) => Instance.LoadPrivacyManager(
                                            campaignType: campaignType, 
                                            pmId: pmId, 
                                            tab: tab);

        public void CustomConsentGDPR(
            string[] vendors, 
            string[] categories, 
            string[] legIntCategories, 
            Action<GdprConsent> onSuccessDelegate) => Instance.CustomConsentGDPR(
                                                        vendors: vendors,
                                                        categories: categories,
                                                        legIntCategories: legIntCategories,
                                                        onSuccessDelegate: onSuccessDelegate);

        public void DeleteCustomConsentGDPR(
            string[] vendors, 
            string[] categories, 
            string[] legIntCategories, 
            Action<GdprConsent> onSuccessDelegate) => Instance.DeleteCustomConsentGDPR(
                                                        vendors: vendors,
                                                        categories: categories,
                                                        legIntCategories: legIntCategories,
                                                        onSuccessDelegate: onSuccessDelegate);

        public SpConsents GetSpConsents() => Instance.GetSpConsents();

        public GdprConsent GetCustomConsent() => Instance.GetCustomConsent();

        public void Dispose() => Instance.Dispose();
        
        #region private methods
        private CMP() {}
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