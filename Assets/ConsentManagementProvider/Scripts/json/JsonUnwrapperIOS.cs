using System;
using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal class JsonUnwrapperIOS
    {
        public static SpConsents UnwrapSpConsents(string json)
        {
            try
            {
                SpConsentsWrapperIOS wrapped = JsonUnwrapperHelper.Deserialize<SpConsentsWrapperIOS>(json);
                
                if (wrapped == null)
                    throw new Newtonsoft.Json.JsonException("JSON deserialization returned null.");

                SpGdprConsent unwrappedGdpr = CMP.Instance.UseGDPR ? UnwrapSpGdprConsent(wrapped.gdpr) : null;
                SpCcpaConsent unwrappedCcpa = CMP.Instance.UseCCPA ? UnwrapSpCcpaConsent(wrapped.ccpa) : null;
                SpUsnatConsent unwrappedUsnat = CMP.Instance.UseUSNAT ? UnwrapSpUsnatConsent(wrapped.usnat) : null;

                return new SpConsents(unwrappedGdpr, unwrappedCcpa, unwrappedUsnat);
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                throw new Newtonsoft.Json.JsonException("Error deserializing JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during JSON unwrapping.", ex);
            }
        }

        private static SpGdprConsent UnwrapSpGdprConsent(SpGdprWrapperIOS wrappedGdpr)
        {
            if (wrappedGdpr == null)
            {
                CmpDebugUtil.LogError("The GDPR consent wrapper cannot be null.");
                return null;
            }
            GdprConsent consent = UnwrapGdprConsent(wrappedGdpr.consents);
            return new SpGdprConsent(consent);
        }

        private static GdprConsent UnwrapGdprConsent(SpGdprConsentWrapperIOS wrapped)
        {
            GdprConsent unwrapped = JsonUnwrapperHelper.UnwrapBaseSpGdprConsent<SpGdprConsentWrapperIOS>(wrapped);
            unwrapped.applies = wrapped.applies;
            
            if (wrapped.grants == null)
                CmpDebugUtil.LogError("The grants dictionary is null.");
            else
                JsonUnwrapperHelper.UnwrapGrantsGdpr(wrapped.grants, ref unwrapped, "vendorGrant");

            if (wrapped.consentStatus != null)
                unwrapped.consentStatus = JsonUnwrapperHelper.UnwrapConsentStatus(wrapped.consentStatus);

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

        private static SpCcpaConsent UnwrapSpCcpaConsent(SpCcpaWrapperIOS wrappedCcpa)
        {
            if (wrappedCcpa == null)
            {
                CmpDebugUtil.LogError("The CCPA consent wrapper cannot be null.");
                return null;
            }
            CcpaConsent consent = UnwrapCcpaConsent(wrappedCcpa.consents);
            return new SpCcpaConsent(consent);
        }

        private static CcpaConsent UnwrapCcpaConsent(SpCcpaConsentWrapperIOS wrapped)
        {
            ConsentStatus consentStatus = JsonUnwrapperHelper.UnwrapConsentStatus(wrapped.consentStatus);

            return new CcpaConsent(uuid: wrapped.uuid,
                                    status: wrapped.status,
                                    uspstring: wrapped.uspstring,
                                    rejectedVendors: wrapped.rejectedVendors,
                                    rejectedCategories: wrapped.rejectedCategories,
                                    childPmId: wrapped.childPmId,
                                    applies: wrapped.applies,
                                    signedLspa: wrapped.signedLspa,
                                    webConsentPayload: wrapped.webConsentPayload,
                                    consentStatus: consentStatus);
        }

        private static SpUsnatConsent UnwrapSpUsnatConsent(SpUsnatWrapperIOS wrappedUsnat)
        {
            if (wrappedUsnat == null)
            {
                CmpDebugUtil.LogError("The USNAT consent wrapper cannot be null.");
                return null;
            }
            UsnatConsent consent = UnwrapUsnatConsent(wrappedUsnat.consents);
            return new SpUsnatConsent(consent);
        }

        private static UsnatConsent UnwrapUsnatConsent(SpUsnatConsentWrapperIOS wrapped)
        {
            ConsentStatus consentStatus = JsonUnwrapperHelper.UnwrapConsentStatus(wrapped.consentStatus);

            JsonUnwrapperHelper.UnwrapUsnatConsents(
                                        wrapped.consentStrings, 
                                        wrapped.vendors, 
                                        wrapped.categories, 
                                        out List<ConsentString> consentStrings, 
                                        out List<Consentable> vendors, 
                                        out List<Consentable> categories);

            return new UsnatConsent(uuid: wrapped.uuid,
                                    applies: wrapped.applies,
                                    consentStrings: consentStrings,
                                    vendors: vendors,
                                    categories: categories,
                                    consentStatus: consentStatus);
        }

        public static GdprConsent UnwrapGdprConsent(string json)
        {
            SpGdprConsentWrapperIOS wrapped = JsonUnwrapperHelper.Deserialize<SpGdprConsentWrapperIOS>(json); 
            GdprConsent unwrapped = UnwrapGdprConsent(wrapped);
            return unwrapped;
        }
    }
}
