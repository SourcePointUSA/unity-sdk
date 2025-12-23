namespace ConsentManagementProvider
{
    public class SpConsents
    {
#nullable enable
        public SpGdprConsent? gdpr;
        public SpCcpaConsent? ccpa;
        public SpUsnatConsent? usnat;
#nullable disable

        public SpConsents(SpGdprConsent gdpr, SpCcpaConsent ccpa, SpUsnatConsent usnat)
        {
            this.gdpr = gdpr;
            this.ccpa = ccpa;
            this.usnat = usnat;
        }
    }
}