using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpGdprConsent
    {
        [JsonInclude] public bool applies;
        [JsonInclude] public GdprConsent consents;

        public SpGdprConsent(bool applies, GdprConsent consents)
        {
            this.applies = applies;
            this.consents = consents;
        }

		public SpGdprConsent(GdprConsent consents)
        {
            this.consents = consents;
        }
    }
}