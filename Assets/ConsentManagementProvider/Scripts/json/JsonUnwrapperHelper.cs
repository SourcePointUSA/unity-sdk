using System;
using System.Collections.Generic;
using System.IO;
using NewtonsoftJson = Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;

namespace ConsentManagementProviderLib.Json
{
    internal static class JsonUnwrapperHelper
    {
        internal static T Deserialize<T>(string json)
        {
            using StringReader stringReader = new StringReader(json);
            using NewtonsoftJson.JsonTextReader reader = new NewtonsoftJson.JsonTextReader(stringReader);
            NewtonsoftJson.JsonSerializer serializer = new NewtonsoftJson.JsonSerializer();
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
            GranularStatus _granularStatus = null;
            if (wrappedconsentStatus.granularStatus != null)
            {
                _granularStatus = new GranularStatus(
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
                granularStatus = _granularStatus,
                hasConsentData = wrappedconsentStatus.hasConsentData,
                rejectedVendors = wrappedconsentStatus.rejectedVendors,
                rejectedCategories = wrappedconsentStatus.rejectedCategories
            };
            return consentStatus;
        }

        internal static void UnwrapUsnatConsents(
            List<ConsentStringWrapper> _consentStringsWrapped, 
            List<ConsentableWrapper> _vendorsWrapped, 
            List<ConsentableWrapper> _categoriesWrapped, 
            out List<ConsentString> _consentStrings, 
            out List<Consentable> _vendors, 
            out List<Consentable> _categories)
        {
            _consentStrings = UnwrapConsentStrings(_consentStringsWrapped);
            _vendors = UnwrapConsentable(_vendorsWrapped);
            _categories = UnwrapConsentable(_categoriesWrapped);
        }

        internal static List<Consentable> UnwrapConsentable(List<ConsentableWrapper> _wrapped)
        {
            List<Consentable> _consentableList= new List<Consentable>();
            foreach (ConsentableWrapper _consentable in _wrapped)
            {
                _consentableList.Add(new Consentable { id = _consentable.id, consented = _consentable.consented });
            }
            return _consentableList;
        }

        internal static List<ConsentString> UnwrapConsentStrings(List<ConsentStringWrapper> _consentStringsWrapped)
        {
            List<ConsentString> _consentStrings = new List<ConsentString>();
            foreach (ConsentStringWrapper _string in _consentStringsWrapped)
            {
                _consentStrings.Add(new ConsentString(_string.consentString, _string.sectionId, _string.sectionName));
            }
            return _consentStrings;
        }
    }
}
