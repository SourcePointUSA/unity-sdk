using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class CMP 
    {
        public static void Initialize(List<SpCampaign> spCampaigns, int accountId, string propertyName, MESSAGE_LANGUAGE language, long messageTimeoutInSeconds = 3)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperV6.Instance.InitializeLib(spCampaigns: spCampaigns,
                                                        accountId: accountId,
                                                        propertyName: propertyName,
                                                        language: language,
                                                        messageTimeoutMilliSeconds: messageTimeoutInSeconds * 1000);
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ConsentWrapperIOS.Instance.InitializeLib(spCampaigns: spCampaigns,
                                                        accountId: accountId,
                                                        propertyName: propertyName,
                                                        language: language,
                                                        messageTimeoutInSeconds: messageTimeoutInSeconds);
            }
#endif
        }

        public static void LoadMessage(string authId = null)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperV6.Instance.LoadMessage(authId: authId);
            }
#elif UNITY_IOS && !UNITY_EDITOR_OSX
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ConsentWrapperIOS.Instance.LoadMessage(authId: authId);
            }
#endif
        }

        public static void LoadPrivacyManager(CAMPAIGN_TYPE campaignType, string pmId, PRIVACY_MANAGER_TAB tab)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperV6.Instance.LoadPrivacyManager(campaignType: campaignType,
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
#endif
        }

        public static void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<SpGdprConsent> onSuccessDelegate)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperV6.Instance.CustomConsentGDPR(vendors: vendors,
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

        public static void Dispose()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ConsentWrapperV6.Instance.Dispose();
            }
#endif
        }
    }
}