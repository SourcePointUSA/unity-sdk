using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpCcpaConsent
    {
        [JsonInclude] public object applies;
        [JsonInclude] public CcpaConsent consents;

        public SpCcpaConsent(bool applies, CcpaConsent consents)
        {
            this.applies = applies;
            this.consents = consents;
        }
        public SpCcpaConsent(CcpaConsent consents)
        {
            this.consents = consents;
        }
    }
}