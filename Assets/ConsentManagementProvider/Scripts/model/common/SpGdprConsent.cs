namespace ConsentManagementProviderLib
{
    public class SpGdprConsent
    {
        public bool? applies => consents?.applies;
        public GdprConsent consents;

		public SpGdprConsent(GdprConsent consents) => this.consents = consents;
    }
}