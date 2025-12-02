using System;
using System.Collections.Generic;
using System.IO;

namespace ConsentManagementProvider.Json
{
    internal class JsonUnwrapperAndroid
    {
        public static SpConsents UnwrapSpConsents(string json)
        {
            try
            {
                SpConsentsWrapperAndroid wrapped = JsonUnwrapperHelper.Deserialize<SpConsentsWrapperAndroid>(json);
                
                if (wrapped == null)
                    throw new Newtonsoft.Json.JsonException("JSON deserialization returned null.");

                SpGdprConsent unwrappedGdpr = CMP.Instance.UseGDPR ? UnwrapSpGdprConsent(wrapped.gdpr) : new SpGdprConsent(new GdprConsent());
                SpCcpaConsent unwrappedCcpa = CMP.Instance.UseCCPA ? UnwrapSpCcpaConsent(wrapped.ccpa) : new SpCcpaConsent(new CcpaConsent());
                SpUsnatConsent unwrappedUsnat = CMP.Instance.UseUSNAT ? UnwrapSpUsnatConsent(wrapped.usnat) : new SpUsnatConsent(new UsnatConsent());

                return new SpConsents(unwrappedGdpr, unwrappedCcpa, unwrappedUsnat);
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                CmpDebugUtil.LogError(ex.Message);
                throw new Newtonsoft.Json.JsonException("Error deserializing JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during JSON unwrapping." + ex.Message, ex);
            }
        }

        public static SpGdprConsent UnwrapSpGdprConsent(SpGdprConsentWrapperAndroid wrappedGdpr)
        {
            if (wrappedGdpr == null)
            {
                CmpDebugUtil.LogError("The GDPR consent wrapper cannot be null.");
                return null;
            }
            
            GdprConsent unwrapped = JsonUnwrapperHelper.UnwrapBaseSpGdprConsent<SpGdprConsentWrapperAndroid>(wrappedGdpr);
            unwrapped.applies = wrappedGdpr.applies;
            
            if (wrappedGdpr.grants == null)
                CmpDebugUtil.LogError("The grants dictionary is null.");
            else
                JsonUnwrapperHelper.UnwrapGrantsGdpr(wrappedGdpr.grants, ref unwrapped, "granted");

            if (wrappedGdpr.consentStatus != null)
                unwrapped.consentStatus = JsonUnwrapperHelper.UnwrapConsentStatus(wrappedGdpr.consentStatus);

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

        public static SpCustomConsentAndroid UnwrapSpCustomConsent(string spConsentsJson)
        {
            try
            {
                SpCustomConsentAndroid customConsent;

                using (StringReader stringReader = new StringReader(spConsentsJson))
                using (Newtonsoft.Json.JsonTextReader jsonReader = new Newtonsoft.Json.JsonTextReader(stringReader))
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    customConsent = serializer.Deserialize<SpCustomConsentAndroid>(jsonReader);

                    if (customConsent == null)
                        throw new InvalidOperationException("Deserialized custom consent is null.");
                }

                return customConsent;
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                throw new Newtonsoft.Json.JsonException("Error deserializing custom consent JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during custom consent JSON unwrapping.", ex);
            }
        }

        private static SpCcpaConsent UnwrapSpCcpaConsent(SpCcpaConsentWrapperAndroid wrappedCcpa)
        {
            if (wrappedCcpa == null)
            {
                CmpDebugUtil.LogError("The CCPA consent wrapper cannot be null.");
                return null;
            }

            ConsentStatus consentStatus = null;
            if (wrappedCcpa.consentStatus != null)
                consentStatus = JsonUnwrapperHelper.UnwrapConsentStatus(wrappedCcpa.consentStatus);

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
                consentStatus: consentStatus,
                GPPData: wrappedCcpa.GPPData);

            return new SpCcpaConsent(unwrapped);
        }

        private static SpUsnatConsent UnwrapSpUsnatConsent(SpUsnatConsentWrapperAndroid wrapped)
        {
            StatusesUsnat statuses = new StatusesUsnat
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
            
            List<ConsentStringWrapper> consentStringsWrapped = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConsentStringWrapper>>(wrapped.consentStrings);
            List<ConsentableWrapper> vendorsWrapped = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConsentableWrapper>>(wrapped.vendors);
            List<ConsentableWrapper> categoriesWrapped = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConsentableWrapper>>(wrapped.categories);

            JsonUnwrapperHelper.UnwrapUsnatConsents(
                                        consentStringsWrapped, 
                                        vendorsWrapped, 
                                        categoriesWrapped, 
                                        out List<ConsentString> consentStrings, 
                                        out List<Consentable> vendors, 
                                        out List<Consentable> categories);

            return new SpUsnatConsent(new UsnatConsent(uuid: wrapped.uuid,
                                    applies: wrapped.applies,
                                    consentStrings: consentStrings,
                                    vendors: vendors,
                                    categories: categories,
                                    statuses: statuses,
                                    GPPData: wrapped.GPPData));
        }
    }
}
