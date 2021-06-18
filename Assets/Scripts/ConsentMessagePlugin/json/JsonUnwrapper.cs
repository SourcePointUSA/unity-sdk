using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class JsonUnwrapper
    {
        public static GdprConsent UnwrapGdprConsent(string json)
        {
            GdprConsentWrapper wrapped = JsonSerializer.Deserialize<GdprConsentWrapper>(json);
            GdprConsent unwrapped = UnwrapGdprConsent(wrapped);
            return unwrapped;
        }

        public static SpConsents UnwrapSpConsents(string json)
        {
            SpConsentsWrapper wrapped = JsonSerializer.Deserialize<SpConsentsWrapper>(json);
            SpGdprConsent unwrappedGdpr = UnwrapSpGdprConsent(wrapped.gdpr);
            SpCcpaConsent unwrappedCcpa = UnwrapSpCcpaConsent(wrapped.ccpa);
            return new SpConsents(unwrappedGdpr, unwrappedCcpa);
        }
        
        private static SpGdprConsent UnwrapSpGdprConsent(SpGdprConsentWrapper wrappedGdpr)
        {
            bool applies = ((JsonElement) wrappedGdpr.applies).GetBoolean();
            GdprConsent consent = UnwrapGdprConsent(wrappedGdpr.consents);
            return new SpGdprConsent (applies, consent);
        }
        
        private static GdprConsent UnwrapGdprConsent(GdprConsentWrapper wrapped)
        {
            GdprConsent unwrapped = new GdprConsent
            {
                euconsent = wrapped.euconsent,
                TCData = wrapped.TCData,
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
            bool applies = ((JsonElement) wrappedCcpa.applies).GetBoolean();
            CcpaConsent consent = UnwrapCcpaConsent(wrappedCcpa.consents);
            return new SpCcpaConsent(applies, consent);
        }

        private static CcpaConsent UnwrapCcpaConsent(CcpaConsentWrapper wrapped)
        {
            return new CcpaConsent(wrapped.status, wrapped.uspstring, wrapped.rejectedVendors, wrapped.rejectedCategories);
        }

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