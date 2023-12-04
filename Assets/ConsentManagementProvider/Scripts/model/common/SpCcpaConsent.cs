namespace ConsentManagementProviderLib
{
    public class SpCcpaConsent
    {
        public object applies;
        public CcpaConsent consents;

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