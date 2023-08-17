using System;
using System.Collections.Generic;
#if UNITY_IOS && !UNITY_EDITOR_OSX
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace ConsentManagementProviderLib.iOS
{
    internal class ConsentWrapperIOS 
    {
        private static ConsentWrapperIOS instance;
        public static ConsentWrapperIOS Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConsentWrapperIOS();
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        private static GameObject IOSListenerGO;
        private static CMPiOSListenerHelper iOSListener;

#if UNITY_IOS && !UNITY_EDITOR_OSX
        [DllImport("__Internal")]
        private static extern void _loadMessage(string authId);
        [DllImport("__Internal")]
        private static extern void _addTargetingParamForCampaignType(int campaignType, string key, string value);
        [DllImport("__Internal")]
        private static extern void _consrtuctLib(int accountId, int propId, string propName, int arrSize, int[] campaignTypes, int campaignsEnvironment, long timeOutSeconds);
        [DllImport("__Internal")]
        private static extern void _setMessageLanguage(int langId);
        [DllImport("__Internal")]
        private static extern void _loadGDPRPrivacyManager(string pmId, int tabId);
        [DllImport("__Internal")]
        private static extern void _loadCCPAPrivacyManager(string pmId, int tabId);
        [DllImport("__Internal")]
        private static extern void _cleanDict();
        [DllImport("__Internal")]
        private static extern void _cleanArrays();
        [DllImport("__Internal")]
        private static extern void _customConsentGDPRWithVendors();
        [DllImport("__Internal")]
        private static extern void _addVendor(string vendor);
        [DllImport("__Internal")]
        private static extern void _addLegIntCategory(string legIntCategory);
        [DllImport("__Internal")]
        private static extern void _addCategory(string category);
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

        public void InitializeLib(List<SpCampaign> spCampaigns, int accountId, int propertyId, string propertyName, MESSAGE_LANGUAGE language, CAMPAIGN_ENV campaignsEnvironment, long messageTimeoutInSeconds = 3)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _cleanDict();
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
            _consrtuctLib(accountId: accountId,
						  propId: propertyId,
                          propName: propertyName,
                          arrSize: campaignsAmount,
                          campaignTypes: campaignTypes,
                          campaignsEnvironment: (int) campaignsEnvironment,
                          timeOutSeconds: messageTimeoutInSeconds);
            _setMessageLanguage((int)language);
#endif
        }

        public void LoadMessage(string authId = null)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadMessage(authId);
#endif
        }

        public void LoadGDPRPrivacyManager(string pmId, PRIVACY_MANAGER_TAB tab)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadGDPRPrivacyManager(pmId, (int)tab);
#endif
        }

        public void LoadCCPAPrivacyManager(string pmId, PRIVACY_MANAGER_TAB tab)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadCCPAPrivacyManager(pmId, (int)tab);
#endif
        }

        public void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _cleanArrays();
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
            _customConsentGDPRWithVendors();
#endif
        }

        public GdprConsent GetCustomGdprConsent()
        {
            return iOSListener.customGdprConsent;
        }

        public SpConsents GetSpConsents()
        {
            return iOSListener._spConsents;
        }
        
        public void Dispose()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _cleanArrays();
            _cleanDict();
#endif
        }
    }
}