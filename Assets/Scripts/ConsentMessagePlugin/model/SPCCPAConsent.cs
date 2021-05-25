using UnityEngine;

namespace ConsentManagementProviderLib
{
    [System.Serializable]
    public class SPCCPAConsent
    {
        public CCPAConsent consent;
        public bool applies = false;

        /*
        public SPCCPAConsent(AndroidJavaObject nativeSpCcpa)
        {
            bool applies = nativeSpCcpa.Call<bool>("getApplies");
            AndroidJavaObject nativeCcpa = nativeSpCcpa.Call<AndroidJavaObject>("getConsent");
            SetFields(new CCPAConsent(nativeCcpa), applies);
        }

        public SPCCPAConsent(CCPAConsent consent, bool applies)
        {
            SetFields(consent, applies);
        }

        private void SetFields(CCPAConsent consent, bool applies)
        {
            this.consent = consent;
            this.applies = applies;
        }
        */
    }
}