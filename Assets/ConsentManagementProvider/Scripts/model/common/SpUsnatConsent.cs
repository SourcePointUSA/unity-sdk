namespace ConsentManagementProviderLib
{
    public class SpUsnatConsent
    {
        public object applies;
        public UsnatConsent consents;

        public SpUsnatConsent(bool applies, UsnatConsent consents)
        {
            this.applies = applies;
            this.consents = consents;
        }
        public SpUsnatConsent(UsnatConsent consents)
        {
            this.consents = consents;
        }
    }
}