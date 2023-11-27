using System;
using System.Collections.Generic;
using System.IO;
using NewtonsoftJson = Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

                SpGdprConsent unwrappedGdpr = wrapped.gdpr != null ? UnwrapSpGdprConsentAndroid(wrapped.gdpr) : null;
                SpCcpaConsent unwrappedCcpa = wrapped.ccpa != null ? UnwrapSpCcpaConsentAndroid(wrapped.ccpa) : null;

                return new SpConsents(unwrappedGdpr, unwrappedCcpa);
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

        private static SpCcpaConsent UnwrapSpCcpaConsentAndroid(CcpaConsentWrapper wrappedCcpa)
        {
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
                throw new ArgumentNullException(nameof(wrappedGdpr), "The GDPR consent wrapper cannot be null.");

            if (wrappedGdpr.grants == null)
                throw new InvalidOperationException("The grants dictionary is null.");

            GdprConsent unwrapped = new GdprConsent
            {
                uuid = wrappedGdpr.uuid,
                euconsent = wrappedGdpr.euconsent,
                TCData = wrappedGdpr.tcData,
                grants = new Dictionary<string, SpVendorGrant>()
            };

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

                SpGdprConsent unwrappedGdpr = wrapped.gdpr != null ? UnwrapSpGdprConsent(wrapped.gdpr) : null;
                SpCcpaConsent unwrappedCcpa = wrapped.ccpa != null ? UnwrapSpCcpaConsent(wrapped.ccpa) : null;

                return new SpConsents(unwrappedGdpr, unwrappedCcpa);
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
                throw new ArgumentNullException(nameof(wrappedGdpr), "The GDPR consent wrapper cannot be null.");
        
            bool applies = wrappedGdpr.applies;
            GdprConsent consent = UnwrapGdprConsent(wrappedGdpr.consents);
            return new SpGdprConsent(applies, consent);
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
                throw new InvalidOperationException("The grants dictionary is null.");

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

            if (wrapped.consentStatus != null){
                unwrapped.consentStatus = UnwrapConsentStatus(wrapped.consentStatus);
            }
            
            return unwrapped;
        }

        private static ConsentStatus UnwrapConsentStatus(ConsentStatusWrapper wrappedconsentStatus)
        {
            GranularStatus granularStatus = null;
            if (wrappedconsentStatus.granularStatus != null){
                granularStatus = new GranularStatus(
                    wrappedconsentStatus.granularStatus.vendorConsent,
                    wrappedconsentStatus.granularStatus.vendorLegInt,
                    wrappedconsentStatus.granularStatus.purposeConsent,
                    wrappedconsentStatus.granularStatus.purposeLegInt,
                    wrappedconsentStatus.granularStatus.previousOptInAll,
                    wrappedconsentStatus.granularStatus.defaultConsent
                );
            }
            ConsentStatus consentStatus = new ConsentStatus(
                wrappedconsentStatus.rejectedAny,
                wrappedconsentStatus.rejectedLI,
                wrappedconsentStatus.consentedAll,
                wrappedconsentStatus.consentedToAny,
                wrappedconsentStatus.vendorListAdditions,
                wrappedconsentStatus.legalBasisChanges,
                wrappedconsentStatus.hasConsentData,
                granularStatus,
                wrappedconsentStatus.rejectedVendors,
                wrappedconsentStatus.rejectedCategories
            );
            return consentStatus;
        }

        private static SpCcpaConsent UnwrapSpCcpaConsent(SpCcpaConsentWrapper wrappedCcpa)
        {
            bool applies = wrappedCcpa.applies;
            CcpaConsent consent = UnwrapCcpaConsent(wrappedCcpa.consents);
            return new SpCcpaConsent(applies, consent);
        }

        private static CcpaConsent UnwrapCcpaConsent(CcpaConsentWrapper wrapped)
        {
            ConsentStatus _consentStatus = UnwrapConsentStatus(wrapped.consentStatus);
            
            return new CcpaConsent(uuid: wrapped.uuid, 
                                    status: wrapped.status,
                                    uspstring: wrapped.uspstring, 
                                    rejectedVendors: wrapped.rejectedVendors, 
                                    rejectedCategories: wrapped.rejectedCategories,
									childPmId:"",
									applies: wrapped.applies,
									signedLspa: null,
                                    webConsentPayload: wrapped.webConsentPayload,
                                    consentStatus: _consentStatus);
        }
        #endregion
    }
}