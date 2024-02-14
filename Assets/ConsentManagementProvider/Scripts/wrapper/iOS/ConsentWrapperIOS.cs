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
        private static extern void _initLib();
        [DllImport("__Internal")]
        private static extern void _addTargetingParamForCampaignType(int campaignType, string key, string value);
        [DllImport("__Internal")]
        private static extern void _configLib(int accountId, int propertyId, string propertyName, bool gdpr, bool ccpa, MESSAGE_LANGUAGE language, string gdprPmId, string ccpaPmId);
        [DllImport("__Internal")]
        private static extern void _loadMessage(string authId);
        [DllImport("__Internal")]
        private static extern void _loadGDPRPrivacyManager();
        [DllImport("__Internal")]
        private static extern void _loadCCPAPrivacyManager();
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

        public void InitializeLib(
            int accountId, 
            int propertyId, 
            string propertyName, 
            bool gdpr, 
            bool ccpa, 
            MESSAGE_LANGUAGE language, 
            string gdprPmId, 
            string ccpaPmId,
            List<SpCampaign> spCampaigns,
            CAMPAIGN_ENV campaignsEnvironment, 
            long messageTimeoutInSeconds = 3)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _initLib();
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
            _configLib(accountId, propertyId, propertyName, gdpr, ccpa, language, gdprPmId, ccpaPmId);
#endif
        }

        public void LoadMessage(string authId = null)
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadMessage(authId);
#endif
        }

        public void LoadGDPRPrivacyManager()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadGDPRPrivacyManager();
#endif
        }

        public void LoadCCPAPrivacyManager()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _loadCCPAPrivacyManager();
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

        public GdprConsent GetCustomGdprConsent()
        {
            return iOSListener.customGdprConsent;
        }

        public SpConsents GetSpConsents()
        {
            return iOSListener._spConsents;
        }
        
        public void ClearAllData()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _cleanConsent();
#endif
        }

        public void Dispose()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _dispose();
#endif
        }
    }
}