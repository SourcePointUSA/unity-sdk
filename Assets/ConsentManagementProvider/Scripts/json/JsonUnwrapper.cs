using System;
using System.Collections.Generic;
using System.IO;
using NewtonsoftJson = Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;

namespace ConsentManagementProviderLib.Json
{
    internal static class JsonUnwrapper
    {
        #region Android 
        public static SpConsents UnwrapSpConsentsAndroid(string json)
        {
            try
            {
                using StringReader stringReader = new StringReader(json);
                using NewtonsoftJson.JsonTextReader reader = new NewtonsoftJson.JsonTextReader(stringReader);
                NewtonsoftJson.JsonSerializer serializer = new NewtonsoftJson.JsonSerializer();
                SpConsentsWrapperAndroid wrapped = serializer.Deserialize<SpConsentsWrapperAndroid>(reader);

                if (wrapped == null)
                    throw new NewtonsoftJson.JsonException("JSON deserialization returned null.");

                SpGdprConsent unwrappedGdpr = CMP.useGDPR ? UnwrapSpGdprConsentAndroid(wrapped.gdpr) : null;
                SpCcpaConsent unwrappedCcpa = CMP.useCCPA ? UnwrapSpCcpaConsentAndroid(wrapped.ccpa) : null;
                SpUsnatConsent unwrappedUsnat = CMP.useUSNAT ? UnwrapSpUsnatConsentAndroid(wrapped.usnat) : null;

                return new SpConsents(unwrappedGdpr, unwrappedCcpa, unwrappedUsnat);
            }
            catch (NewtonsoftJson.JsonException ex)
            {
                CmpDebugUtil.LogError(ex.Message);
                throw new ApplicationException("Error deserializing JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during JSON unwrapping." + ex.Message, ex);
            }
        }

        private static SpUsnatConsent UnwrapSpUsnatConsentAndroid(SpUsnatConsentWrapperAndroid wrapped)
        {
            StatusesUsnat _statuses = new StatusesUsnat
            {
                hasConsentData = wrapped.statuses.hasConsentData,
                rejectedAny = wrapped.statuses.rejectedAny,
                consentedToAll = wrapped.statuses.consentedToAll,
                consentedToAny = wrapped.statuses.consentedToAny,
                sellStatus = wrapped.statuses.sellStatus,
                shareStatus = wrapped.statuses.shareStatus,
                sensitiveDataStatus = wrapped.statuses.sensitiveDataStatus,
                gpcStatus = wrapped.statuses.gpcStatus
            };
            List<ConsentString> _consentStrings = new List<ConsentString>();
            List<ConsentStringWrapper> _consentStringsWrapped = NewtonsoftJson.JsonConvert.DeserializeObject<List<ConsentStringWrapper>>(wrapped.consentStrings);
            foreach (ConsentStringWrapper _string in _consentStringsWrapped)
            {
                _consentStrings.Add(new ConsentString(_string.consentString, _string.sectionId, _string.sectionName));
            }

            List<Consentable> _vendors = new List<Consentable>();
            List<ConsentableWrapper> _vendorsWrapped = NewtonsoftJson.JsonConvert.DeserializeObject<List<ConsentableWrapper>>(wrapped.vendors);
            foreach (ConsentableWrapper _consentable in _vendorsWrapped)
            {
                _vendors.Add(new Consentable { id = _consentable.id, consented = _consentable.consented });
            }

            List<Consentable> _categories = new List<Consentable>();
            List<ConsentableWrapper> _categoriesWrapped = NewtonsoftJson.JsonConvert.DeserializeObject<List<ConsentableWrapper>>(wrapped.categories);
            foreach (ConsentableWrapper _consentable in _categoriesWrapped)
            {
                _categories.Add(new Consentable { id = _consentable.id, consented = _consentable.consented });
            }
            return new SpUsnatConsent(new UsnatConsent(uuid: wrapped.uuid,
                                    applies: wrapped.applies,
                                    consentStrings: _consentStrings,
                                    vendors: _vendors,
                                    categories: _categories,
                                    statuses: _statuses));
        }

        private static SpCcpaConsent UnwrapSpCcpaConsentAndroid(CcpaConsentWrapper wrappedCcpa)
        {
            if (wrappedCcpa == null)
            {
                CmpDebugUtil.LogError("The CCPA consent wrapper cannot be null.");
                return null;
            }
            CcpaConsent unwrapped = new CcpaConsent(
                uuid: wrappedCcpa.uuid,
                status: wrappedCcpa.status,
                uspstring: wrappedCcpa.uspstring,
                rejectedVendors: wrappedCcpa.rejectedVendors,
                rejectedCategories: wrappedCcpa.rejectedCategories,
                childPmId: wrappedCcpa.childPmId,
                applies: wrappedCcpa.applies,
                signedLspa: wrappedCcpa.signedLspa,
                webConsentPayload: wrappedCcpa.webConsentPayload,
                null
            );

            return new SpCcpaConsent(unwrapped);
        }

        public static SpGdprConsent UnwrapSpGdprConsentAndroid(SpGdprConsentWrapperAndroid wrappedGdpr)
        {
            if (wrappedGdpr == null)
            {
                CmpDebugUtil.LogError("The GDPR consent wrapper cannot be null.");
                return null;
            }

            GdprConsent unwrapped = new GdprConsent
            {
                uuid = wrappedGdpr.uuid,
                euconsent = wrappedGdpr.euconsent,
                TCData = wrappedGdpr.tcData,
                grants = new Dictionary<string, SpVendorGrant>()
            };

            if (wrappedGdpr.grants == null)
            {
                CmpDebugUtil.LogError("The grants dictionary is null.");
            }
            else
            {
                foreach (var vendorGrantWrapper in wrappedGdpr.grants)
                {
                    var purposeGrants = new Dictionary<string, bool>();
                    bool isGranted = false;

                    var vendorGrantValue = JToken.FromObject(vendorGrantWrapper.Value);

                    if (vendorGrantValue["granted"] != null)
                        isGranted = vendorGrantValue["granted"].ToObject<bool>();

                    if (vendorGrantValue["purposeGrants"] != null)
                    {
                        var purposeGrantsElement = (JObject)vendorGrantValue["purposeGrants"];

                        foreach (var purposeGrant in purposeGrantsElement)
                            purposeGrants.Add(purposeGrant.Key, purposeGrant.Value.ToObject<bool>());
                    }
                    unwrapped.grants[vendorGrantWrapper.Key] = new SpVendorGrant(isGranted, purposeGrants);
                }
            }

            if (wrappedGdpr.gcmStatus != null)
            {
                unwrapped.googleConsentMode = new SPGCMData(
                    wrappedGdpr.gcmStatus.ad_storage,
                    wrappedGdpr.gcmStatus.analytics_storage,
                    wrappedGdpr.gcmStatus.ad_user_data,
                    wrappedGdpr.gcmStatus.ad_personalization
                );
            }

            return new SpGdprConsent(unwrapped);
        }

        public static SpCustomConsentAndroid UnwrapSpCustomConsentAndroid(string spConsentsJson)
        {
            try
            {
                SpCustomConsentAndroid customConsent;

                using (StringReader stringReader = new StringReader(spConsentsJson))
                using (NewtonsoftJson.JsonTextReader jsonReader = new NewtonsoftJson.JsonTextReader(stringReader))
                {
                    NewtonsoftJson.JsonSerializer serializer = new NewtonsoftJson.JsonSerializer();
                    customConsent = serializer.Deserialize<SpCustomConsentAndroid>(jsonReader);

                    if (customConsent == null)
                    {
                        throw new InvalidOperationException("Deserialized custom consent is null.");
                    }
                }

                return customConsent;
            }
            catch (NewtonsoftJson.JsonException ex)
            {
                throw new ApplicationException("Error deserializing custom consent JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during custom consent JSON unwrapping.", ex);
            }
        }
        #endregion

        #region IOS
        public static GdprConsent UnwrapGdprConsent(string json)
        {
            using StringReader stringReader = new StringReader(json);
            using NewtonsoftJson.JsonTextReader reader = new NewtonsoftJson.JsonTextReader(stringReader);

            NewtonsoftJson.JsonSerializer serializer = new NewtonsoftJson.JsonSerializer();
            GdprConsentWrapper wrapped = serializer.Deserialize<GdprConsentWrapper>(reader);

            GdprConsent unwrapped = UnwrapGdprConsent(wrapped);
            return unwrapped;
        }

        public static SpConsents UnwrapSpConsents(string json)
        {
            try
            {
                using StringReader stringReader = new StringReader(json);
                using NewtonsoftJson.JsonTextReader reader = new NewtonsoftJson.JsonTextReader(stringReader);

                NewtonsoftJson.JsonSerializer serializer = new NewtonsoftJson.JsonSerializer();
                SpConsentsWrapper wrapped = serializer.Deserialize<SpConsentsWrapper>(reader);

                if (wrapped == null)
                    throw new NewtonsoftJson.JsonException("JSON deserialization returned null.");

                SpGdprConsent unwrappedGdpr = CMP.useGDPR ? UnwrapSpGdprConsent(wrapped.gdpr) : null;
                SpCcpaConsent unwrappedCcpa = CMP.useCCPA ? UnwrapSpCcpaConsent(wrapped.ccpa) : null;
                SpUsnatConsent unwrappedUsnat = CMP.useUSNAT ? UnwrapSpUsnatConsent(wrapped.usnat) : null;

                return new SpConsents(unwrappedGdpr, unwrappedCcpa, unwrappedUsnat);
            }
            catch (NewtonsoftJson.JsonException ex)
            {
                throw new ApplicationException("Error deserializing JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during JSON unwrapping.", ex);
            }
        }

        private static SpGdprConsent UnwrapSpGdprConsent(SpGdprConsentWrapper wrappedGdpr)
        {
            if (wrappedGdpr == null)
            {
                CmpDebugUtil.LogError("The GDPR consent wrapper cannot be null.");
                return null;
            }
            GdprConsent consent = UnwrapGdprConsent(wrappedGdpr.consents);
            return new SpGdprConsent(consent);
        }

        private static GdprConsent UnwrapGdprConsent(GdprConsentWrapper wrapped)
        {
            GdprConsent unwrapped = new GdprConsent
            {
                applies = wrapped.applies,
                uuid = wrapped.uuid,
                webConsentPayload = wrapped.webConsentPayload,
                euconsent = wrapped.euconsent,
                TCData = wrapped.TCData,
                grants = new Dictionary<string, SpVendorGrant>()
            };

            if (wrapped.grants == null)
            {
                CmpDebugUtil.LogError("The grants dictionary is null.");
            }
            else
            {
                foreach (var vendorGrantWrapper in wrapped.grants)
                {
                    var purposeGrants = new Dictionary<string, bool>();
                    bool isGranted = false;

                    var vendorGrantValue = JToken.FromObject(vendorGrantWrapper.Value);

                    if (vendorGrantValue["granted"] != null)
                        isGranted = vendorGrantValue["granted"].ToObject<bool>();

                    if (vendorGrantValue["purposeGrants"] != null)
                    {
                        var purposeGrantsElement = (JObject)vendorGrantValue["purposeGrants"];

                        foreach (var purposeGrant in purposeGrantsElement)
                            purposeGrants.Add(purposeGrant.Key, purposeGrant.Value.ToObject<bool>());
                    }

                    unwrapped.grants[vendorGrantWrapper.Key] = new SpVendorGrant(isGranted, purposeGrants);
                }
            }

            if (wrapped.consentStatus != null)
            {
                unwrapped.consentStatus = UnwrapConsentStatus(wrapped.consentStatus);
            }
            
            if (wrapped.gcmStatus != null)
            {
                unwrapped.googleConsentMode = new SPGCMData(
                    wrapped.gcmStatus.ad_storage,
                    wrapped.gcmStatus.analytics_storage,
                    wrapped.gcmStatus.ad_user_data,
                    wrapped.gcmStatus.ad_personalization
                );
            }
            
            return unwrapped;
        }

        private static ConsentStatus UnwrapConsentStatus(ConsentStatusWrapper wrappedconsentStatus)
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

        private static SpCcpaConsent UnwrapSpCcpaConsent(SpCcpaConsentWrapper wrappedCcpa)
        {
            if (wrappedCcpa == null)
            {
                CmpDebugUtil.LogError("The CCPA consent wrapper cannot be null.");
                return null;
            }
            CcpaConsent consent = UnwrapCcpaConsent(wrappedCcpa.consents);
            return new SpCcpaConsent(consent);
        }

        private static CcpaConsent UnwrapCcpaConsent(CcpaConsentWrapper wrapped)
        {
            ConsentStatus _consentStatus = UnwrapConsentStatus(wrapped.consentStatus);

            return new CcpaConsent(uuid: wrapped.uuid,
                                    status: wrapped.status,
                                    uspstring: wrapped.uspstring,
                                    rejectedVendors: wrapped.rejectedVendors,
                                    rejectedCategories: wrapped.rejectedCategories,
                                    childPmId: "",
                                    applies: wrapped.applies,
                                    signedLspa: null,
                                    webConsentPayload: wrapped.webConsentPayload,
                                    consentStatus: _consentStatus);
        }

        private static SpUsnatConsent UnwrapSpUsnatConsent(SpUsnatConsentWrapper wrappedUsnat)
        {
            if (wrappedUsnat == null)
            {
                CmpDebugUtil.LogError("The USNAT consent wrapper cannot be null.");
                return null;
            }
            UsnatConsent consent = UnwrapUsnatConsent(wrappedUsnat.consents);
            return new SpUsnatConsent(consent);
        }

        private static UsnatConsent UnwrapUsnatConsent(UsnatConsentWrapper wrapped)
        {
            ConsentStatus _consentStatus = UnwrapConsentStatus(wrapped.consentStatus);

            List<ConsentString> _consentStrings = new List<ConsentString>();
            foreach (ConsentStringWrapper _string in wrapped.consentStrings)
            {
                _consentStrings.Add(new ConsentString(_string.consentString, _string.sectionId, _string.sectionName));
            }
            List<Consentable> _vendors = new List<Consentable>();
            foreach (ConsentableWrapper _consentable in wrapped.vendors)
            {
                _vendors.Add(new Consentable{id = _consentable.id, consented = _consentable.consented});
            }
            List<Consentable> _categories = new List<Consentable>();
            foreach (ConsentableWrapper _consentable in wrapped.categories)
            {
                _categories.Add(new Consentable{id = _consentable.id, consented = _consentable.consented});
            }

            return new UsnatConsent(uuid: wrapped.uuid,
                                    applies: wrapped.applies,
                                    consentStrings: _consentStrings,
                                    vendors: _vendors,
                                    categories: _categories,
                                    consentStatus: _consentStatus);
        }
        #endregion
    }
}