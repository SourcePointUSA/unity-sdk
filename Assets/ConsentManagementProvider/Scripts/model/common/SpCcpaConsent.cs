namespace ConsentManagementProvider
{
    public class SpCcpaConsent
    {
        public bool? applies => consents?.applies;
        public CcpaConsent consents;

        public SpCcpaConsent(CcpaConsent consents) => this.consents = consents;
    }
}