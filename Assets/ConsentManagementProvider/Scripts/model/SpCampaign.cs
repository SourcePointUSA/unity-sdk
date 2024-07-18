using System.Collections.Generic;

namespace ConsentManagementProvider
{
    public class SpCampaign
    {
        private CAMPAIGN_TYPE campaignType;
        private List<TargetingParam> targetingParams;
        private bool transitionCCPAAuth = false;
        private bool supportLegacyUSPString = false;
        
        public CAMPAIGN_TYPE CampaignType { get => campaignType; }
        public List<TargetingParam> TargetingParams { get => targetingParams; }
        public bool TransitionCCPAAuth { get => transitionCCPAAuth; }
        public bool SupportLegacyUSPString { get => supportLegacyUSPString; }

        public SpCampaign(
            CAMPAIGN_TYPE campaignType, 
            List<TargetingParam> targetingParams, 
            bool transitionCCPAAuth = false,
            bool supportLegacyUSPString = false)
        {
            this.campaignType = campaignType;
            this.targetingParams = targetingParams;
            this.transitionCCPAAuth = transitionCCPAAuth;
            this.supportLegacyUSPString = supportLegacyUSPString;
        }
    }
}