using System.Collections.Generic;

namespace ConsentManagementProviderLib
{
    public class SpCampaign
    {
        private CAMPAIGN_TYPE campaignType;
        private List<TargetingParam> targetingParams;

        public CAMPAIGN_TYPE CampaignType { get => campaignType; }
        public List<TargetingParam> TargetingParams { get => targetingParams; }

        public SpCampaign(CAMPAIGN_TYPE campaignType, List<TargetingParam> targetingParams)
        {
            this.campaignType = campaignType;
            this.targetingParams = targetingParams;
        }
    }
}