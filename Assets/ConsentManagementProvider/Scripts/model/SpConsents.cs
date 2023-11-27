namespace ConsentManagementProviderLib
{
    public class SpConsents
    {
        public SpGdprConsent gdpr;
        public SpCcpaConsent ccpa;

        public SpConsents(SpGdprConsent gdpr, SpCcpaConsent ccpa)
        {
            this.gdpr = gdpr;
            this.ccpa = ccpa;
        }
    }
}