using UnityEngine;

namespace GdprConsentLib
{
    public class SpConsents
    {
        private SPGDPRConsent gdpr;
        private SPCCPAConsent ccpa;
        //private AndroidJavaObject nativeConsents;

        public SPGDPRConsent GDPR
        {
            get
            {
                return gdpr;
            }
            private set
            {
                gdpr = value;
            }
        }

        public SPCCPAConsent CCPA
        {
            get
            {
                return ccpa;
            }
            private set
            {
                ccpa = value;
            }
        }
        /*
        public SpConsents(AndroidJavaObject spConsents)
        {
            //this.nativeConsents = spConsents;
            Util.Log("SpConsents constructor starts...");
            AndroidJavaObject nativeSpGdpr = spConsents.Call<AndroidJavaObject>("getGdpr");
            AndroidJavaObject nativeSpCcpa = spConsents.Call<AndroidJavaObject>("getCcpa");
            Util.Log("SpConsents nativeSpGdpr starts...");
            var a = new SPGDPRConsent(nativeSpGdpr);
            Util.Log("SpConsents nativeSpCcpa starts...");
            var b = new SPCCPAConsent(nativeSpCcpa);
            Util.Log("SpConsents SET starts...");
            SetConsents(a, b);
            Util.Log("SpConsents constructor ended!");
        }

        public SpConsents(SPGDPRConsent gdpr, SPCCPAConsent ccpa)
        {
            SetConsents(gdpr, ccpa);
        }

        private void SetConsents(SPGDPRConsent gdpr, SPCCPAConsent ccpa)
        {
            this.gdpr = gdpr;
            this.ccpa = ccpa;
        }
        */
    }
}