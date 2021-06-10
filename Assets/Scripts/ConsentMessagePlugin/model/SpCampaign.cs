using System.Collections.Generic;

namespace ConsentManagementProviderLib
{
    public class SpCampaign
    {
        private CAMPAIGN_TYPE campaignType;
        private CAMPAIGN_ENV environment;
        private List<TargetingParam> targetingParams;
        
        public CAMPAIGN_TYPE CampaignType { get => campaignType; }
        public List<TargetingParam> TargetingParams { get => targetingParams; }
        public CAMPAIGN_ENV Environment { get => environment; }

        public SpCampaign(CAMPAIGN_TYPE campaignType, CAMPAIGN_ENV environment, List<TargetingParam> targetingParams)
        {
            this.campaignType = campaignType;
            this.environment = environment;
            this.targetingParams = targetingParams;
        }
    }
}