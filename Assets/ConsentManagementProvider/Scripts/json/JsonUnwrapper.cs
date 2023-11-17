using System;
using System.Collections.Generic;
using System.Text.Json;
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
            bool applies = ((JsonElement) wrappedGdpr.applies).GetBoolean();
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
                unwrapped.grants[vendorGrantWrapper.Key] = new SpVendorGrant(isGranted, purposeGrants);
            }

            JsonElement granularStatusWrapped = getValueJsonElement((JsonElement)wrapped.consentStatus, "granularStatus");
            unwrapped.consentStatus = UnwrapConsentStatus(granularStatusWrapped, wrapped.consentStatus);

            return unwrapped;
        }

        private static ConsentStatus UnwrapConsentStatus(JsonElement granularStatusWrapped, object wrappedconsentStatus)
        {
            GranularStatus granularStatusUnwrapped;
            if (granularStatusWrapped.ValueKind != JsonValueKind.Undefined)
            {
                granularStatusUnwrapped = new GranularStatus(
                    getStringJsonElement((JsonElement)granularStatusWrapped, "vendorConsent"),
                    getStringJsonElement((JsonElement)granularStatusWrapped, "vendorLegInt"),
                    getStringJsonElement((JsonElement)granularStatusWrapped, "purposeConsent"),
                    getStringJsonElement((JsonElement)granularStatusWrapped, "purposeLegInt"),
                    getBoolJsonElement((JsonElement)granularStatusWrapped, "previousOptInAll"),
                    getBoolJsonElement((JsonElement)granularStatusWrapped, "defaultConsent")
                );
            }
            else
            {
                granularStatusUnwrapped = null;
            }

            ConsentStatus consentStatus = new ConsentStatus(
                getBoolJsonElement((JsonElement)wrappedconsentStatus, "rejectedAny"),
                getBoolJsonElement((JsonElement)wrappedconsentStatus, "rejectedLI"),
                getBoolJsonElement((JsonElement)wrappedconsentStatus, "consentedAll"),
                getBoolJsonElement((JsonElement)wrappedconsentStatus, "consentedToAny"),
                getBoolJsonElement((JsonElement)wrappedconsentStatus, "vendorListAdditions"),
                getBoolJsonElement((JsonElement)wrappedconsentStatus, "legalBasisChanges"),
                getBoolJsonElement((JsonElement)wrappedconsentStatus, "hasConsentData"),
                granularStatusUnwrapped,
                getObjectJsonElement((JsonElement)wrappedconsentStatus, "rejectedVendors"),
                getObjectJsonElement((JsonElement)wrappedconsentStatus, "rejectedCategories")
            );
            return consentStatus;
        }

        private static JsonElement getValueJsonElement(JsonElement element, string name)
        {
            if (element.TryGetProperty(name, out JsonElement value))
            {
                return value;
            }
            return new JsonElement();
        }
        
        private static bool? getBoolJsonElement(JsonElement element, string name)
        {
            JsonElement value = getValueJsonElement(element, name);
            if (value.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }
            return value.GetBoolean();
        }
        
        private static string? getStringJsonElement(JsonElement element, string name)
        {
            JsonElement value = getValueJsonElement(element, name);
            if (value.ValueKind == JsonValueKind.Undefined)
            {
                CmpDebugUtil.LogWarning(name + " == JsonValueKind.Undefined! Unable to unwrap!");
                return null;
            }
            return value.ToString();
        }
        
        private static object getObjectJsonElement(JsonElement element, string name)
        {
            JsonElement value = getValueJsonElement(element, name);
            if (value.ValueKind == JsonValueKind.Undefined)
            {
                CmpDebugUtil.LogWarning(name + " == JsonValueKind.Undefined! Unable to unwrap!");
                return null;
            }
            return value;
        }

        private static SpCcpaConsent UnwrapSpCcpaConsent(SpCcpaConsentWrapper wrappedCcpa)
        {
            bool applies = ((JsonElement)wrappedCcpa.applies).GetBoolean();
            CcpaConsent consent = UnwrapCcpaConsent(wrappedCcpa.consents);
            return new SpCcpaConsent(applies, consent);
        }

        private static CcpaConsent UnwrapCcpaConsent(CcpaConsentWrapper wrapped)
        {
            JsonElement granularStatusWrapped = getValueJsonElement((JsonElement)wrapped.consentStatus, "granularStatus");
            ConsentStatus _consentStatus = UnwrapConsentStatus(granularStatusWrapped, wrapped.consentStatus);
            
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