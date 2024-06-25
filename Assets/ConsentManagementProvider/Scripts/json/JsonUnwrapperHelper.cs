using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ConsentManagementProviderLib.Json
{
    internal static class JsonUnwrapperHelper
    {
        internal static T Deserialize<T>(string json)
        {
            StringReader stringReader = new StringReader(json);
            Newtonsoft.Json.JsonTextReader reader = new Newtonsoft.Json.JsonTextReader(stringReader);
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            return serializer.Deserialize<T>(reader);
        }

        internal static GdprConsent UnwrapBaseSpGdprConsent<T>(T wrapped) where T : GdprConsentWrapper
        {
            GdprConsent unwrapped = new GdprConsent
            {
                uuid = wrapped.uuid,
                webConsentPayload = wrapped.webConsentPayload,
                euconsent = wrapped.euconsent,
                TCData = wrapped.TCData,
                grants = new Dictionary<string, SpVendorGrant>()
            };
            return unwrapped;
        }

        internal static void UnwrapGrantsGdpr(Dictionary<string, SpVendorGrantWrapper> grants, ref GdprConsent unwrapped, string isGrantedString)
        {
            foreach (var vendorGrantWrapper in grants)
            {
                var purposeGrants = new Dictionary<string, bool>();
                bool isGranted = false;

                var vendorGrantValue = JToken.FromObject(vendorGrantWrapper.Value);

                if (vendorGrantValue[isGrantedString] != null)
                    isGranted = vendorGrantValue[isGrantedString].ToObject<bool>();

                if (vendorGrantValue["purposeGrants"] != null)
                {
                    var purposeGrantsElement = (JObject)vendorGrantValue["purposeGrants"];

                    foreach (var purposeGrant in purposeGrantsElement)
                        purposeGrants.Add(purposeGrant.Key, purposeGrant.Value.ToObject<bool>());
                }
                
                unwrapped.grants[vendorGrantWrapper.Key] = new SpVendorGrant(isGranted, purposeGrants);
            }
        }

        internal static ConsentStatus UnwrapConsentStatus(ConsentStatusWrapper wrappedconsentStatus)
        {
            GranularStatus granularStatus = null;
            
            if (wrappedconsentStatus.granularStatus != null)
            {
                granularStatus = new GranularStatus(
                    wrappedconsentStatus.granularStatus.vendorConsent,
                    wrappedconsentStatus.granularStatus.vendorLegInt,
                    wrappedconsentStatus.granularStatus.purposeConsent,
                    wrappedconsentStatus.granularStatus.purposeLegInt,
                    wrappedconsentStatus.granularStatus.previousOptInAll,
                    wrappedconsentStatus.granularStatus.defaultConsent,
                    wrappedconsentStatus.granularStatus.sellStatus,
                    wrappedconsentStatus.granularStatus.shareStatus,
                    wrappedconsentStatus.granularStatus.sensitiveDataStatus,
                    wrappedconsentStatus.granularStatus.gpcStatus
                );
            }
            
            ConsentStatus consentStatus = new ConsentStatus{
                rejectedAny = wrappedconsentStatus.rejectedAny,
                rejectedLI = wrappedconsentStatus.rejectedLI,
                consentedAll = wrappedconsentStatus.consentedAll,
                consentedToAll = wrappedconsentStatus.consentedToAll,
                consentedToAny = wrappedconsentStatus.consentedToAny,
                rejectedAll = wrappedconsentStatus.rejectedAll,
                vendorListAdditions = wrappedconsentStatus.vendorListAdditions,
                legalBasisChanges = wrappedconsentStatus.legalBasisChanges,
                granularStatus = granularStatus,
                hasConsentData = wrappedconsentStatus.hasConsentData,
                rejectedVendors = wrappedconsentStatus.rejectedVendors,
                rejectedCategories = wrappedconsentStatus.rejectedCategories
            };
            
            return consentStatus;
        }

        internal static void UnwrapUsnatConsents(
            List<ConsentStringWrapper> consentStringsWrapped, 
            List<ConsentableWrapper> vendorsWrapped, 
            List<ConsentableWrapper> categoriesWrapped, 
            out List<ConsentString> consentStrings, 
            out List<Consentable> vendors, 
            out List<Consentable> categories)
        {
            consentStrings = UnwrapConsentStrings(consentStringsWrapped);
            vendors = UnwrapConsentable(vendorsWrapped);
            categories = UnwrapConsentable(categoriesWrapped);
        }

        private static List<Consentable> UnwrapConsentable(List<ConsentableWrapper> wrapped)
        {
            List<Consentable> consentableList = new List<Consentable>();
            
            foreach (ConsentableWrapper consentable in wrapped)
                consentableList.Add(new Consentable { id = consentable.id, consented = consentable.consented });
            
            return consentableList;
        }

        private static List<ConsentString> UnwrapConsentStrings(List<ConsentStringWrapper> consentStringsWrapped)
        {
            List<ConsentString> consentStrings = new List<ConsentString>();
            
            foreach (ConsentStringWrapper str in consentStringsWrapped)
                consentStrings.Add(new ConsentString(str.consentString, str.sectionId, str.sectionName));
            
            return consentStrings;
        }
    }
}
