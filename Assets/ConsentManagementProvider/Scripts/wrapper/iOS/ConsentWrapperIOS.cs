using System;
using System.Collections.Generic;
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
        [DllImport("__Internal")] //TO-DO: add messageTimeoutInSeconds to call
        private static extern void _configLib(int accountId, int propertyId, string propertyName, bool gdpr, bool ccpa, bool usnat, int language, string gdprPmId, string ccpaPmId, string usnatPmId);
        [DllImport("__Internal")]
        private static extern void _loadMessage();
        [DllImport("__Internal")]
        private static extern void _loadMessageWithAuthId(string authId);
        [DllImport("__Internal")] //TO-DO: add pmId, tab to call
        private static extern void _loadGDPRPrivacyManager();
        [DllImport("__Internal")] //TO-DO: add pmId, tab to call
        private static extern void _loadCCPAPrivacyManager();
        [DllImport("__Internal")] //TO-DO: add pmId, tab to call
        private static extern void _loadUSNATPrivacyManager();
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

        private static void CreateHelperIOSListener()
        {
            IOSListenerGO = new GameObject();
            iOSListener = IOSListenerGO.AddComponent<CMPiOSListenerHelper>();
        }

        public void Initialize(
            int accountId, 
            int propertyId, 
            string propertyName, 
            MESSAGE_LANGUAGE language,
            List<SpCampaign> spCampaigns,
            CAMPAIGN_ENV campaignsEnvironment, 
            long messageTimeoutInSeconds = 3,
            bool? transitionCCPAAuth = null,
            bool? supportLegacyUSPString = null)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _initLib();
            if(iOSListener == null)
            {
                CmpDebugUtil.Log("Creating iosListener");
                CreateHelperIOSListener();
            }

            int campaignsAmount = spCampaigns.Count;
            int[] campaignTypes = new int[campaignsAmount];
            foreach(SpCampaign sp in spCampaigns)
            {
                foreach(TargetingParam tp in sp.TargetingParams)
                {
                    _addTargetingParamForCampaignType((int)sp.CampaignType, tp.Key, tp.Value);
                }
            }
            for (int i=0; i<campaignsAmount; i++)
            {
                campaignTypes[i] = (int)spCampaigns[i].CampaignType;
            }
            if(transitionCCPAAuth != null)
                _setTransitionCCPAAuth((bool)transitionCCPAAuth);
            if(supportLegacyUSPString != null)
                _setSupportLegacyUSPString((bool)supportLegacyUSPString);
            _configLib(accountId, propertyId, propertyName, CMP.useGDPR, CMP.useCCPA, CMP.useUSNAT, (int)language, gdprPmId, ccpaPmId, usnatPmId); //TO-DO: add messageTimeoutInSeconds to call
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
            //TO-DO: add pmId, tab to call
            switch (campaignType){
                case CAMPAIGN_TYPE.GDPR: LoadGDPRPrivacyManager(); break;
                case CAMPAIGN_TYPE.CCPA: LoadCCPAPrivacyManager(); break;
                case CAMPAIGN_TYPE.USNAT: LoadUSNATPrivacyManager(); break;
            }
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

        public void LoadGDPRPrivacyManager()
        {
            //TO-DO: add pmId, tab to call
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadGDPRPrivacyManager();
#endif
        }

        public void LoadCCPAPrivacyManager()
        {
            //TO-DO: add pmId, tab to call
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadCCPAPrivacyManager();
#endif
        }

        public void LoadUSNATPrivacyManager()
        {
            //TO-DO: add pmId, tab to call
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadUSNATPrivacyManager();
#endif
        }
    }
}