using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpConsents
    {
        [JsonInclude] public SpGdprConsent gdpr;
        [JsonInclude] public SpCcpaConsent ccpa;

        public SpConsents(SpGdprConsent gdpr, SpCcpaConsent ccpa)
        {
            this.gdpr = gdpr;
            this.ccpa = ccpa;
        }
    }
}