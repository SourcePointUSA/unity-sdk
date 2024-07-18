namespace ConsentManagementProvider
{
    public class SpConsents
    {
        public SpGdprConsent gdpr;
        public SpCcpaConsent ccpa;
        public SpUsnatConsent usnat;

        public SpConsents(SpGdprConsent gdpr, SpCcpaConsent ccpa, SpUsnatConsent usnat)
        {
            this.gdpr = gdpr;
            this.ccpa = ccpa;
            this.usnat = usnat;
        }
    }
}