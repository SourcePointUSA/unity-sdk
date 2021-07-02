using System;
using System.Collections.Generic;
using ConsentManagementProviderLib.Android;
using ConsentManagementProviderLib.iOS;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class CMP
    {
        public static void Initialize(List<SpCampaign> spCampaigns, int accountId, string propertyName, MESSAGE_LANGUAGE language, CAMPAIGN_ENV campaignsEnvironment, long messageTimeoutInSeconds = 3)
        {
#if UNITY_ANDROID
            //TODO  campaignsEnvironment
            if (Application.platform == RuntimePlatform.Android)
            {
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
#endif
        }

        public static void LoadMessage(string authId = null)
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
#endif
        }

        public static void LoadPrivacyManager(CAMPAIGN_TYPE campaignType, string pmId, PRIVACY_MANAGER_TAB tab)
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
#endif
        }

        public static void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate)
        {
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
#endif
        }
    }
}