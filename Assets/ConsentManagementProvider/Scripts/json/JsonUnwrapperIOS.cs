using System;
using System.Collections.Generic;

namespace ConsentManagementProviderLib.Json
{
    internal static class JsonUnwrapperIOS
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
                throw new ApplicationException("Error deserializing JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during JSON unwrapping.", ex);
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
            {
                unwrapped.consentStatus = JsonUnwrapperHelper.UnwrapConsentStatus(wrapped.consentStatus);
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
            ConsentStatus _consentStatus = JsonUnwrapperHelper.UnwrapConsentStatus(wrapped.consentStatus);

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
            ConsentStatus _consentStatus = JsonUnwrapperHelper.UnwrapConsentStatus(wrapped.consentStatus);

            List<ConsentString> _consentStrings;
            List<Consentable> _vendors, _categories;
            JsonUnwrapperHelper.UnwrapUsnatConsents(
                                        wrapped.consentStrings, 
                                        wrapped.vendors, 
                                        wrapped.categories, 
                                        out _consentStrings, 
                                        out _vendors, 
                                        out _categories);

            return new UsnatConsent(uuid: wrapped.uuid,
                                    applies: wrapped.applies,
                                    consentStrings: _consentStrings,
                                    vendors: _vendors,
                                    categories: _categories,
                                    consentStatus: _consentStatus);
        }

        public static GdprConsent UnwrapGdprConsent(string json)
        {
            SpGdprConsentWrapperIOS wrapped = JsonUnwrapperHelper.Deserialize<SpGdprConsentWrapperIOS>(json); 
            GdprConsent unwrapped = UnwrapGdprConsent(wrapped);
            return unwrapped;
        }
    }
}
