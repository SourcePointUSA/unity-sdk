namespace ConsentManagementProviderLib
{
    public class SpGdprConsent
    {
        public bool applies;
        public GdprConsent consents;

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