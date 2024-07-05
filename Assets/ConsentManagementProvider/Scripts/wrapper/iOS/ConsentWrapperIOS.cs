using System;
using System.Collections.Generic;
using ConsentManagementProviderLib.Enum;

#if UNITY_IOS && !UNITY_EDITOR_OSX
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace ConsentManagementProviderLib.iOS
{
    internal class ConsentWrapperIOS: ISpSdk
    {
        private static GameObject IOSListenerGO;
        private static CMPiOSListenerHelper iOSListener;

#if UNITY_IOS && !UNITY_EDITOR_OSX
        [DllImport("__Internal")]
        private static extern void _initLib();
        [DllImport("__Internal")]
        private static extern void _addTargetingParamForCampaignType(int campaignType, string key, string value);
        [DllImport("__Internal")]
        private static extern void _setTransitionCCPAAuth(bool value);
        [DllImport("__Internal")]
        private static extern void _setSupportLegacyUSPString(bool value);
        [DllImport("__Internal")]
        private static extern void _configLib(int accountId, int propertyId, string propertyName, bool gdpr, bool ccpa, bool usnat, string language, long messageTimeoutInSeconds);
        [DllImport("__Internal")]
        private static extern void _loadMessage();
        [DllImport("__Internal")]
        private static extern void _loadMessageWithAuthId(string authId);
        [DllImport("__Internal")]
        private static extern void _loadPrivacyManager(int campaignType, string pmId, int tab);
        [DllImport("__Internal")]
        private static extern void _cleanConsent();
        [DllImport("__Internal")]
        private static extern void _customConsentGDPR();
        [DllImport("__Internal")]
        private static extern void _deleteCustomConsentGDPR();
        [DllImport("__Internal")]
        private static extern void _addVendor(string vendor);
        [DllImport("__Internal")]
        private static extern void _addCategory(string category);
        [DllImport("__Internal")]
        private static extern void _addLegIntCategory(string legIntCategory);
        [DllImport("__Internal")]
        private static extern void _clearCustomArrays();
        [DllImport("__Internal")]
        private static extern void _dispose();
#endif

        public ConsentWrapperIOS()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            CreateHelperIOSListener();
#endif
        }

        public void Initialize(
            int accountId, 
            int propertyId, 
            string propertyName, 
            MESSAGE_LANGUAGE language,
            List<SpCampaign> spCampaigns,
            CAMPAIGN_ENV campaignsEnvironment, 
            long messageTimeoutInSeconds)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _initLib();
            if(iOSListener == null)
            {
                CmpDebugUtil.Log("Creating iosListener");
                CreateHelperIOSListener();
            }
            
            bool transitionCCPAAuth = false;
            bool supportLegacyUSPString = false;
            int campaignsAmount = spCampaigns.Count;
            int[] campaignTypes = new int[campaignsAmount];
            foreach(SpCampaign sp in spCampaigns)
            {
                foreach(TargetingParam tp in sp.TargetingParams)
                {
                    _addTargetingParamForCampaignType((int)sp.CampaignType, tp.Key, tp.Value);
                }
                if (sp.CampaignType == CAMPAIGN_TYPE.USNAT)
                {
                    transitionCCPAAuth = sp.TransitionCCPAAuth;
                    supportLegacyUSPString = sp.SupportLegacyUSPString;
                }
            }
            for (int i=0; i<campaignsAmount; i++)
            {
                campaignTypes[i] = (int)spCampaigns[i].CampaignType;
            }
            if(transitionCCPAAuth)
                _setTransitionCCPAAuth(transitionCCPAAuth);
            if(supportLegacyUSPString)
                _setSupportLegacyUSPString(supportLegacyUSPString);

            string langName = CMPEnumMapper.GetMessageLanguageKey(language);
            _configLib(accountId, propertyId, propertyName, CMP.Instance.UseGDPR, CMP.Instance.UseCCPA, CMP.Instance.UseUSNAT, langName, messageTimeoutInSeconds);
#endif
        }

        public void LoadMessage(string authId = null)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            if (string.IsNullOrEmpty(authId))
            {
                CmpDebugUtil.Log("Calling load message without authId");
                _loadMessage();
            }
            else 
            {
                CmpDebugUtil.Log($"Calling load message with authId={authId}");
                _loadMessageWithAuthId(authId);
            }
#endif
        }

        public void LoadPrivacyManager(CAMPAIGN_TYPE campaignType, string pmId, PRIVACY_MANAGER_TAB tab)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadPrivacyManager((int)campaignType, pmId, (int)tab);
#endif
        }

        public void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _clearCustomArrays();
            foreach (string vendor in vendors)
            {
                _addVendor(vendor);
            }
            foreach (string category in categories)
            { 
                _addCategory(category);
            }
            foreach (string legInt in legIntCategories)
            {
                _addLegIntCategory(legInt);
            }
            iOSListener.SetCustomConsentsGDPRSuccessAction(onSuccessDelegate);
            _customConsentGDPR();
#endif
        }

        public void DeleteCustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _clearCustomArrays();
            foreach (string vendor in vendors)
            {
                _addVendor(vendor);
            }
            foreach (string category in categories)
            { 
                _addCategory(category);
            }
            foreach (string legInt in legIntCategories)
            {
                _addLegIntCategory(legInt);
            }
            iOSListener.SetCustomConsentsGDPRSuccessAction(onSuccessDelegate);
            _deleteCustomConsentGDPR();
#endif
        }

        public SpConsents GetSpConsents() => iOSListener._spConsents;

        public GdprConsent GetCustomConsent() => iOSListener.customGdprConsent;
        
        public void ClearAllData()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            iOSListener._spConsents = null;
            _cleanConsent();
#endif
        }

        public void Dispose()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _dispose();
            iOSListener.Dispose();
#endif
        }

        private static void CreateHelperIOSListener()
        {
            IOSListenerGO = new GameObject();
            iOSListener = IOSListenerGO.AddComponent<CMPiOSListenerHelper>();
        }
    }
}