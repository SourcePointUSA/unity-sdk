using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConsentManagementProviderLib.UnityEditor
{
    public class ConsentWrapperUnityEditor: ISpSdk
    {
        public ConsentWrapperUnityEditor() =>
            Debug.LogWarning("ATTENTION! Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");

        public void Initialize(int accountId, int propertyId, string propertyName, MESSAGE_LANGUAGE language, List<SpCampaign> spCampaigns, CAMPAIGN_ENV campaignsEnvironment, long messageTimeoutInSeconds = 3) =>
            Debug.LogWarning("Emulating InitializeLib call.. Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");

        public void LoadMessage(string authId = null) =>
            Debug.LogWarning("Emulating LoadMessage call... Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");

        public void LoadPrivacyManager(CAMPAIGN_TYPE campaignType, string pmId, PRIVACY_MANAGER_TAB tab) =>
            Debug.LogWarning($"Emulating LoadPrivacyManager call for {campaignType}... " +
                             $"Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");

        public void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate) =>
            Debug.LogWarning("Emulating CustomConsentGDPR call.. Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");

        public void DeleteCustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate) =>
            Debug.LogWarning("Emulating DeleteCustomConsentGDPR call.. Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");

        public SpConsents GetSpConsents()
        {
            Debug.LogWarning("Emulating GetSpConsents call.. Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");
            return null;
        }

        public GdprConsent GetCustomConsent()
        {
            Debug.LogWarning("Emulating GetCustomConsent call.. Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");
            return null;
        }

        public void ClearAllData() =>
            Debug.LogWarning("Emulating ClearAllData call... Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");

        public void Dispose() =>
            Debug.LogWarning("Emulating Dispose call.. Sourcepoint CMP works only for real Android/iOS devices, not the Unity Editor.");
    }
}