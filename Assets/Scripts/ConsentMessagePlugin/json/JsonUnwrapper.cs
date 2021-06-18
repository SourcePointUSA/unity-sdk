using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class JsonUnwrapper
    {
        public static SpGdprConsent UnwrapSpGdprConsent(SpGdprConsentWrapper wrapped)
        {
            SpGdprConsent unwrapped = new SpGdprConsent
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

        private static void PrintGrants(SpGdprConsent unwrapped)
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