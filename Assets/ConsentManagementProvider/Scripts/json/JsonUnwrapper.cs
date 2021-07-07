using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace ConsentManagementProviderLib.Json
{
    internal static class JsonUnwrapper
    {
        #region Android 
        public static SpConsents UnwrapSpConsentsAndroid(string json)
        {
            SpConsentsWrapperAndroid wrapped = JsonSerializer.Deserialize<SpConsentsWrapperAndroid>(json);
            SpGdprConsent unwrappedGdpr = null;
            SpCcpaConsent unwrappedCcpa = null;
            if (wrapped.gdpr!= null)
            {
                unwrappedGdpr = UnwrapSpGdprConsentAndroid(wrapped.gdpr);
            }
            if (wrapped.ccpa != null)
            {
                unwrappedCcpa = UnwrapSpCcpaConsentAndroid(wrapped.ccpa);
            }
            return new SpConsents(unwrappedGdpr, unwrappedCcpa);
        }

        private static SpCcpaConsent UnwrapSpCcpaConsentAndroid(CcpaConsentWrapper wrappedCcpa)
        {
            CcpaConsent unwrapped = new CcpaConsent(uuid: wrappedCcpa.uuid,
                                                    status: wrappedCcpa.status,
                                                    uspstring: wrappedCcpa.uspstring,
                                                    rejectedVendors: wrappedCcpa.rejectedVendors,
                                                    rejectedCategories: wrappedCcpa.rejectedCategories);
            return new SpCcpaConsent(unwrapped);
        }

        public static SpGdprConsent UnwrapSpGdprConsentAndroid(SpGdprConsentWrapperAndroid wrappedGdpr)
        {
            GdprConsent unwrapped = new GdprConsent
            {
                uuid = wrappedGdpr.uuid,
                euconsent = wrappedGdpr.euconsent,
                TCData = wrappedGdpr.tcData,       
                grants = new Dictionary<string, SpVendorGrant>()
            };
            foreach (KeyValuePair<string, Dictionary<string, object>> vendorGrantWrapper in wrappedGdpr.grants)
            {
                Dictionary<string, bool> purposeGrants = new Dictionary<string, bool>();
                if (vendorGrantWrapper.Value != null)
                {
                    foreach (KeyValuePair<string, object> purpGrant in vendorGrantWrapper.Value) 
                    {
                        purposeGrants.Add(purpGrant.Key, ((JsonElement)purpGrant.Value).GetBoolean());
                    }
                }
                unwrapped.grants[vendorGrantWrapper.Key] = new SpVendorGrant(/*isGranted,*/ purposeGrants);
            }
            return new SpGdprConsent(unwrapped);
        }
        
        public static SpCustomConsentAndroid UnwrapSpCustomConsentAndroid(string spConsentsJson)
        {
            SpCustomConsentAndroid customConsent = JsonSerializer.Deserialize<SpCustomConsentAndroid>(spConsentsJson);
            return customConsent;
        }
        #endregion

        #region IOS
        public static GdprConsent UnwrapGdprConsent(string json)
        {
            GdprConsentWrapper wrapped = JsonSerializer.Deserialize<GdprConsentWrapper>(json);
            GdprConsent unwrapped = UnwrapGdprConsent(wrapped);
            return unwrapped;
        }

        public static SpConsents UnwrapSpConsents(string json)
        {
            SpConsentsWrapper wrapped = JsonSerializer.Deserialize<SpConsentsWrapper>(json);
            SpGdprConsent unwrappedGdpr = null;
            if (wrapped.gdpr != null)
            {
                unwrappedGdpr = UnwrapSpGdprConsent(wrapped.gdpr);
            }
            SpCcpaConsent unwrappedCcpa = null;
            if (wrapped.ccpa != null)
            {
                unwrappedCcpa = UnwrapSpCcpaConsent(wrapped.ccpa);
            }
            return new SpConsents(unwrappedGdpr, unwrappedCcpa);
        }
        
        private static SpGdprConsent UnwrapSpGdprConsent(SpGdprConsentWrapper wrappedGdpr)
        {
            // bool applies = ((JsonElement) wrappedGdpr.applies).GetBoolean();
            GdprConsent consent = UnwrapGdprConsent(wrappedGdpr.consents);
            return new SpGdprConsent(consent);
        }
        
        private static GdprConsent UnwrapGdprConsent(GdprConsentWrapper wrapped)
        {
            GdprConsent unwrapped = new GdprConsent
            {
                uuid = wrapped.uuid,
                euconsent = wrapped.euconsent,
                TCData = wrapped.TCData,
                grants = new Dictionary<string, SpVendorGrant>()
            };
            foreach (KeyValuePair<string, SpVendorGrantWrapper> vendorGrantWrapper in wrapped.grants)
            {
                bool isGranted = ((JsonElement)vendorGrantWrapper.Value.vendorGrant).GetBoolean();
                Dictionary<string, bool> purposeGrants = new Dictionary<string, bool>();
                if (vendorGrantWrapper.Value != null)
                {
                    foreach (KeyValuePair<string, object> purpGrant in vendorGrantWrapper.Value.purposeGrants)
                    {
                        purposeGrants.Add(purpGrant.Key, ((JsonElement)purpGrant.Value).GetBoolean());
                    }
                }
                unwrapped.grants[vendorGrantWrapper.Key] = new SpVendorGrant(/*isGranted,*/ purposeGrants);
            }
            return unwrapped;
        }

        private static SpCcpaConsent UnwrapSpCcpaConsent(SpCcpaConsentWrapper wrappedCcpa)
        {
            //bool applies = ((JsonElement)wrappedCcpa.applies).GetBoolean();
            CcpaConsent consent = UnwrapCcpaConsent(wrappedCcpa.consents);
            return new SpCcpaConsent(consent);
        }

        private static CcpaConsent UnwrapCcpaConsent(CcpaConsentWrapper wrapped)
        {
            return new CcpaConsent(uuid: wrapped.uuid,
                                   status: wrapped.status, 
                                   uspstring: wrapped.uspstring, 
                                   rejectedVendors: wrapped.rejectedVendors, 
                                   rejectedCategories: wrapped.rejectedCategories);
        }
        #endregion
    }
}