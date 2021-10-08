﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using ConsentManagementProviderLib.Android;
using ConsentManagementProviderLib.iOS;
using ConsentManagementProviderLib.Observer;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class CMP
    {
        private static GameObject mainThreadBroadcastEventsExecutorGO;
        private static BroadcastEventsExecutor mainThreadBroadcastEventsExecutor;
        private static MESSAGE_LANGUAGE language;
        
        public static void Initialize(List<SpCampaign> spCampaigns, 
                                      int accountId, 
                                      string propertyName, 
                                      MESSAGE_LANGUAGE language, 
                                      CAMPAIGN_ENV campaignsEnvironment, 
                                      long messageTimeoutInSeconds = 3)
        {
            if(!IsSpCampaignsValid(spCampaigns))
            { 
                return;
            }
            CreateBroadcastExecutorGO();
            CMP.language = language;
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                //excluding ios14 campaign if any
                RemoveIos14SpCampaign(ref spCampaigns);
                if (!IsSpCampaignsValid(spCampaigns))
                {
                    return;
                }
                ConsentWrapperAndroid.Instance.InitializeLib(spCampaigns: spCampaigns,
                                                            accountId: accountId,
                                                            propertyName: propertyName,
                                                            language: language,
                                                            campaignsEnvironment: campaignsEnvironment,
                                                            messageTimeoutMilliSeconds: messageTimeoutInSeconds * 1000);
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ConsentWrapperIOS.Instance.InitializeLib(spCampaigns: spCampaigns,
                                                        accountId: accountId,
                                                        propertyName: propertyName,
                                                        language: language,
                                                        campaignsEnvironment: campaignsEnvironment,
                                                        messageTimeoutInSeconds: messageTimeoutInSeconds);
            }
#else
            SingleCampaignPostGetMessagesRequest gdprTargetingParams = null;
            SpCampaign gdpr = spCampaigns.Find(x => x.CampaignType == CAMPAIGN_TYPE.GDPR);
            if (gdpr != null)
            {
                Dictionary<string, string> tarParams = new Dictionary<string, string>();
                foreach (var param in gdpr.TargetingParams)
                {
                    tarParams[param.Key] = param.Value;
                }
                gdprTargetingParams = new SingleCampaignPostGetMessagesRequest(tarParams);
            }
            SingleCampaignPostGetMessagesRequest ccpaTargetingParams = null;
            SpCampaign ccpa = spCampaigns.Find(x => x.CampaignType == CAMPAIGN_TYPE.CCPA);
            if (ccpa != null)
            {
                Dictionary<string, string> tarParams = new Dictionary<string, string>();
                foreach (var param in ccpa.TargetingParams)
                {
                    tarParams[param.Key] = param.Value;
                }
                ccpaTargetingParams = new SingleCampaignPostGetMessagesRequest(tarParams);
            }
            CmpLocalizationMapper.GetMessages(accountId: accountId,
                                              propertyHref:propertyName,
                                              gdpr: gdprTargetingParams, 
                                              ccpa: ccpaTargetingParams,
                                              environment: (int) campaignsEnvironment,
                                              millisTimeout: Convert.ToInt32(messageTimeoutInSeconds*1000));
#endif
        }
        
        public static void LoadMessage(string authId = null, GameObject cmpHomePrefab = null, Canvas canvas = null)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperAndroid.Instance.LoadMessage(authId: authId);
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ConsentWrapperIOS.Instance.LoadMessage(authId: authId);
            }
#else
            InstantiateHomePrefab(cmpHomePrefab, canvas);
#endif
        }

        public static void LoadPrivacyManager(CAMPAIGN_TYPE campaignType, string pmId, PRIVACY_MANAGER_TAB tab, GameObject cmpHomePrefab = null, Canvas canvas = null)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperAndroid.Instance.LoadPrivacyManager(campaignType: campaignType,
                                                                 pmId: pmId,
                                                                 tab: tab);
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            { 
                if(campaignType == CAMPAIGN_TYPE.GDPR)
                {
                    ConsentWrapperIOS.Instance.LoadGDPRPrivacyManager(pmId: pmId, 
                                                                      tab: tab);
                }
                else if(campaignType == CAMPAIGN_TYPE.CCPA)
                {
                    ConsentWrapperIOS.Instance.LoadCCPAPrivacyManager(pmId: pmId,
                                                                      tab: tab);
                }
            }
#else
            if (campaignType == CAMPAIGN_TYPE.GDPR)
            {
                CmpLocalizationMapper.MessageGdpr();
                InstantiateHomePrefab(cmpHomePrefab, canvas);
            }
            else
            {
                //TODO: CCPA
            }
#endif
        }

        public static void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate)
        {
            //TODO: check if CustomConsentGdpr feature is available for Unity Native message style
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperAndroid.Instance.CustomConsentGDPR(vendors: vendors,
                                                                categories: categories,
                                                                legIntCategories: legIntCategories,
                                                                onSuccessDelegate: onSuccessDelegate);
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ConsentWrapperIOS.Instance.CustomConsentGDPR(vendors: vendors,
                                                           categories: categories,
                                                           legIntCategories: legIntCategories,
                                                           onSuccessDelegate: onSuccessDelegate);
            }
#endif
        }

        public static SpConsents GetSpConsents()
        {
            SpConsents result = null;
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                result = ConsentWrapperAndroid.Instance.GetSpConsents();
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                result = ConsentWrapperIOS.Instance.GetSpConsents();
            }
#endif
            return result;
        }

        public static GdprConsent GetCustomConsent()
        {
            GdprConsent result = null;
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                result = ConsentWrapperAndroid.Instance.GetCustomGdprConsent();
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                result = ConsentWrapperIOS.Instance.GetCustomGdprConsent();
            }
#endif
            return result;
        }

        public static void Dispose()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperAndroid.Instance.Dispose();
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ConsentWrapperIOS.Instance.Dispose();
            }
#else
            CmpPopupDestroyer.DestroyAllPopups();            
#endif
        }
        
        #region private methods
        private static void InstantiateHomePrefab(GameObject cmpHomePrefab, Canvas canvas)
        {
            if (cmpHomePrefab == null || canvas == null)
            {
                Debug.LogError("To use CMP UI on non-mobile platforms you have to provide CmpHomePrefab and Canvas!");
                return;
            }
            else
            {
                CmpLocalizationMapper.SetCanvas(canvas);
                mainThreadBroadcastEventsExecutor.InstantiateHomeView(cmpHomePrefab, canvas);
                //TODO: authId? 
            }
        }
        
        private static void CreateBroadcastExecutorGO()
        {
            if (mainThreadBroadcastEventsExecutorGO != null) return;
            mainThreadBroadcastEventsExecutorGO = new GameObject();
            mainThreadBroadcastEventsExecutor = mainThreadBroadcastEventsExecutorGO.AddComponent<BroadcastEventsExecutor>();
        }

        private static void RemoveIos14SpCampaign(ref List<SpCampaign> spCampaigns)
        {
            List<SpCampaign> ios14 = spCampaigns.Where(campaign => campaign.CampaignType == CAMPAIGN_TYPE.IOS14).ToList();
            if (ios14 != null && ios14.Count > 0)
            {
                Debug.LogWarning("ios14 campaign is not allowed in non-ios device! Skipping it...");
                foreach (SpCampaign ios in ios14)
                {
                    spCampaigns.Remove(ios);
                }
            }
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