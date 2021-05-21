using GdprConsentLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GdprConsentLib
{
    [CreateAssetMenu(fileName = "SpCampaign", menuName = "new SpCampaign", order = 51)]
    public class SpCampaignScriptableObject : ScriptableObject
    {
        [SerializeField]
        public CAMPAIGN_TYPE campaignTypeToLoad;
        [SerializeField]
        public List<TargetingParamScriptableObject> targetingParams;
    }

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