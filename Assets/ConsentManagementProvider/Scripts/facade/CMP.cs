using System;
using System.Collections.Generic;
using ConsentManagementProvider.Android;
using ConsentManagementProvider.iOS;
using ConsentManagementProvider.Observer;
using ConsentManagementProvider.UnityEditor;
using UnityEngine;

namespace ConsentManagementProvider
{
    public class CMP: ICmpSdk
    {
        public static ICmpSdk Instance => instance ??= new CMP();

        public bool UseGDPR { get; private set; }
        public bool UseCCPA { get; private set; }
        public bool UseUSNAT { get; private set; }
        public bool UseIOS14 { get; private set; }

        private GameObject mainThreadBroadcastEventsExecutor;
        private static ISpSdk concreteInstance;
        private static ICmpSdk instance;
        
        internal static ISpSdk ConcreteInstance
        {
            get
            {
                if (concreteInstance == null)
                    concreteInstance = Application.platform switch
                    {
                        RuntimePlatform.Android => new ConsentWrapperAndroid(),
                        RuntimePlatform.IPhonePlayer => new ConsentWrapperIOS(),
                        _ => new ConsentWrapperUnityEditor(),
                    };
                return concreteInstance;
            }
        }

        public void Initialize(
            int accountId,
            int propertyId,
            string propertyName,
            MESSAGE_LANGUAGE language,
            List<SpCampaign> spCampaigns, 
            CAMPAIGN_ENV campaignsEnvironment,
            long messageTimeoutInSeconds = 3)
        {
            if (!IsSpCampaignsValid(spCampaigns))
                return;

            if (!IsPropertyNameValid(ref propertyName))
                return;

            foreach (SpCampaign sp in spCampaigns)
            {
                switch (sp.CampaignType)
                {
                    case CAMPAIGN_TYPE.GDPR: UseGDPR = true; break;
                    case CAMPAIGN_TYPE.CCPA: UseCCPA = true; break;
                    case CAMPAIGN_TYPE.USNAT: UseUSNAT = true; break;
                    case CAMPAIGN_TYPE.IOS14: UseIOS14 = true; break;
                }
            }

            CreateBroadcastExecutorGameObject();

            ConcreteInstance.Initialize(
                accountId: accountId,
                propertyId: propertyId,
                propertyName: propertyName,
                language: language,
                spCampaigns: spCampaigns,
                campaignsEnvironment: campaignsEnvironment,
                messageTimeoutInSeconds: messageTimeoutInSeconds);
        }
        
        public void LoadMessage(string authId = null) => ConcreteInstance.LoadMessage(authId: authId);

        public void ClearAllData() => ConcreteInstance.ClearAllData();

        public void LoadPrivacyManager(
            CAMPAIGN_TYPE campaignType, 
            string pmId, 
            PRIVACY_MANAGER_TAB tab)
        {
            if (!IsCampaignSetUp(campaignType))
            {
                CmpDebugUtil.LogError($"Campaign {campaignType} was not setup properly. Aborting...");
                return;
            }

            if (string.IsNullOrEmpty(pmId))
            {
                CmpDebugUtil.LogError($"pmID is null or empty for campaign {campaignType}. Aborting...");
                return;
            }

            ConcreteInstance.LoadPrivacyManager(
                campaignType: campaignType,
                pmId: pmId,
                tab: tab);
        }

        public void CustomConsentGDPR(
            string[] vendors, 
            string[] categories, 
            string[] legIntCategories, 
            Action<GdprConsent> onSuccessDelegate) => ConcreteInstance.CustomConsentGDPR(
                                                        vendors: vendors,
                                                        categories: categories,
                                                        legIntCategories: legIntCategories,
                                                        onSuccessDelegate: onSuccessDelegate);

        public void DeleteCustomConsentGDPR(
            string[] vendors, 
            string[] categories, 
            string[] legIntCategories, 
            Action<GdprConsent> onSuccessDelegate) => ConcreteInstance.DeleteCustomConsentGDPR(
                                                        vendors: vendors,
                                                        categories: categories,
                                                        legIntCategories: legIntCategories,
                                                        onSuccessDelegate: onSuccessDelegate);

        public SpConsents GetSpConsents() => ConcreteInstance.GetSpConsents();

        public GdprConsent GetCustomConsent() => ConcreteInstance.GetCustomConsent();

        public void Dispose() => ConcreteInstance.Dispose();
        
        #region private methods
        private void CreateBroadcastExecutorGameObject()
        {
            if (mainThreadBroadcastEventsExecutor != null) return;
            mainThreadBroadcastEventsExecutor = new GameObject();
            mainThreadBroadcastEventsExecutor.AddComponent<BroadcastEventsExecutor>();
        }

        private bool IsSpCampaignsValid(List<SpCampaign> spCampaigns)
        {
            //check if there more than 0 campaign
            if (spCampaigns is null || spCampaigns.Count == 0)
            {
                CmpDebugUtil.LogError("You should add at least one SpCampaign to use CMP! Aborting...");
                return false;
            }
            
            return true;
        }

        private bool IsPropertyNameValid(ref string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrWhiteSpace(propertyName))
            {
                CmpDebugUtil.LogError("Property Name can`t be null or empty! Aborting...");
                return false;
            }
            propertyName = propertyName.Trim();
            return true;
        }            
        
        private bool IsCampaignSetUp(CAMPAIGN_TYPE campaignType)
            {
                switch (campaignType)
                {
                    case CAMPAIGN_TYPE.GDPR: return UseGDPR;
                    case CAMPAIGN_TYPE.CCPA: return UseCCPA;
                    case CAMPAIGN_TYPE.USNAT: return UseUSNAT;
                    case CAMPAIGN_TYPE.IOS14: return UseIOS14;
                    default: return false;
                }
            }
        #endregion
    }
}