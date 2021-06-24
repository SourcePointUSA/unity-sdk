using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace ConsentManagementProviderLib.Json
{
    internal static class JsonUnwrapper
    {
        // ccpa - status, uspstring, rejectedVendors, rejectedCategories
        // gdpr - euconsent, grants

        public static GdprConsent UnwrapGdprConsent(string json)
        {
            GdprConsentWrapper wrapped = JsonSerializer.Deserialize<GdprConsentWrapper>(json);
            GdprConsent unwrapped = UnwrapGdprConsent(wrapped);
            return unwrapped;
        }

        #region Android 
        public static SpConsents UnwrapSpConsentsAndroid(string json)
        {
            SpConsentsWrapperAndroid wrapped = JsonSerializer.Deserialize<SpConsentsWrapperAndroid>(json);
            SpGdprConsent unwrappedGdpr = UnwrapSpGdprConsentAndroid(wrapped.gdpr);
            SpCcpaConsent unwrappedCcpa = UnwrapSpCcpaConsentAndroid(wrapped.ccpa);
            return new SpConsents(unwrappedGdpr, unwrappedCcpa);
        }

        private static SpCcpaConsent UnwrapSpCcpaConsentAndroid(SpCcpaConsentWrapperAndroid wrappedCcpa)
        {
            // bool rejectedAll = ((JsonElement) wrappedCcpa.rejectedAll).GetBoolean();
            CcpaConsent unwrapped = new CcpaConsent(status: wrappedCcpa.status,
                                                    uspstring: wrappedCcpa.uspstring,
                                                    rejectedVendors: wrappedCcpa.rejectedVendors,
                                                    rejectedCategories: wrappedCcpa.rejectedCategories);
            return new SpCcpaConsent(unwrapped);
        }

        public static SpGdprConsent UnwrapSpGdprConsentAndroid(SpGdprConsentWrapperAndroid wrappedGdpr)
        {
            // bool consentedToAll = ((JsonElement) wrappedGdpr.consentedToAll).GetBoolean();
            GdprConsent unwrapped = new GdprConsent
            {
                euconsent = wrappedGdpr.euconsent,
                // TCData = wrappedGdpr.TCData,
                grants = new Dictionary<string, SpVendorGrant>()
            };
            foreach (KeyValuePair<string, SpVendorGrantWrapper> vendorGrantWrapper in wrappedGdpr.grants)
            {
                bool isGranted = ((JsonElement) vendorGrantWrapper.Value.vendorGrant).GetBoolean();
                Dictionary<string, bool> purposeGrants = new Dictionary<string, bool>();
                foreach (KeyValuePair<string, object> purpGrant in vendorGrantWrapper.Value.purposeGrants)
                {
                    purposeGrants.Add(purpGrant.Key, ((JsonElement) purpGrant.Value).GetBoolean());
                }
                unwrapped.grants[vendorGrantWrapper.Key] = new SpVendorGrant(isGranted, purposeGrants);
            }
            return new SpGdprConsent(unwrapped);
        }
        #endregion

        #region IOS
        public static SpConsents UnwrapSpConsents(string json)
        {
            SpConsentsWrapper wrapped = JsonSerializer.Deserialize<SpConsentsWrapper>(json);
            SpGdprConsent unwrappedGdpr = UnwrapSpGdprConsent(wrapped.gdpr);
            SpCcpaConsent unwrappedCcpa = UnwrapSpCcpaConsent(wrapped.ccpa);
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
                euconsent = wrapped.euconsent,
                // TCData = wrapped.TCData,
                grants = new Dictionary<string, SpVendorGrant>()
            };
            foreach (KeyValuePair<string, SpVendorGrantWrapper> vendorGrantWrapper in wrapped.grants)
            {
                bool isGranted = ((JsonElement) vendorGrantWrapper.Value.vendorGrant).GetBoolean();
                Dictionary<string, bool> purposeGrants = new Dictionary<string, bool>();
                foreach (KeyValuePair<string, object> purpGrant in vendorGrantWrapper.Value.purposeGrants)
                {
                    purposeGrants.Add(purpGrant.Key, ((JsonElement) purpGrant.Value).GetBoolean());
                }
                unwrapped.grants[vendorGrantWrapper.Key] = new SpVendorGrant(isGranted, purposeGrants);
            }
            return unwrapped;
        }

        private static SpCcpaConsent UnwrapSpCcpaConsent(SpCcpaConsentWrapper wrappedCcpa)
        {
            // bool applies = ((JsonElement) wrappedCcpa.applies).GetBoolean();
            CcpaConsent consent = UnwrapCcpaConsent(wrappedCcpa.consents);
            return new SpCcpaConsent(consent);
        }

        private static CcpaConsent UnwrapCcpaConsent(CcpaConsentWrapper wrapped)
        {
            return new CcpaConsent(status: wrapped.status, 
                                   uspstring: wrapped.uspstring, 
                                   rejectedVendors: wrapped.rejectedVendors, 
                                   rejectedCategories: wrapped.rejectedCategories);
        }
        #endregion

        private static void PrintGrants(GdprConsent unwrapped)
        {
            var grants = unwrapped.grants;
            foreach (var k in grants.Keys)
            {
                Debug.Log("-----");
                Debug.Log($"purposes for {k} are granted? = {grants[k].vendorGrant}");
                foreach (var j in grants[k].purposeGrants.Keys)
                {
                    Debug.Log(j + "   " + grants[k].purposeGrants[j]);
                }
            }
        }
    }
}