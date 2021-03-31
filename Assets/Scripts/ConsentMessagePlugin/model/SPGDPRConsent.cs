using UnityEngine;

namespace GdprConsentLib
{
    public class SPGDPRConsent
    {
        GDPRConsent consent;
        bool applies = false;

        public SPGDPRConsent(AndroidJavaObject nativeSpGdpr)
        {
            Util.LogError("SPGDPRConsent constructor starts...");
            bool applies = nativeSpGdpr.Call<bool>("getApplies");
            Util.LogError("SPGDPRConsent constructor applies: "+ applies);
            AndroidJavaObject nativeGdpr = nativeSpGdpr.Call<AndroidJavaObject>("getConsent");
            Util.LogError("SPGDPRConsent constructor nativeGdpr");
            SetFields(new GDPRConsent(nativeGdpr), applies);
            Util.LogError("SPGDPRConsent constructor SetFields");
        }

        public SPGDPRConsent(GDPRConsent consent, bool applies)
        {
            SetFields(consent, applies);
        }

        private void SetFields(GDPRConsent consent, bool applies)
        {
            this.consent = consent;
            this.applies = applies;
        }
    }
}