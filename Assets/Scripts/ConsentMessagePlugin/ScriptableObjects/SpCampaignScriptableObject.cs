using GdprConsentLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpCampaign", menuName = "new SpCampaign", order = 51)]
public class SpCampaignScriptableObject : ScriptableObject
{
    [SerializeField]
    public CAMPAIGN_TYPE campaignTypeToLoad;
    [SerializeField]
    public List<TargetingParamScriptableObject> targetingParams;
}