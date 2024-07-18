namespace ConsentManagementProvider
{
    public class SpUsnatConsent
    {
        public bool? applies => consents?.applies;
        public UsnatConsent consents;

        public SpUsnatConsent(UsnatConsent consents) => this.consents = consents;
    }
}