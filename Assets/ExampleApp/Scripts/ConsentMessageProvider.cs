using ConsentManagementProviderLib;
using System.Collections.Generic;
using UnityEngine;

public class ConsentMessageProvider : MonoBehaviour
{
    [SerializeField]
    List<CAMPAIGN_TYPE> allCampaignTypesToLoad;
    [SerializeField]
    MESSAGE_LANGUAGE language = MESSAGE_LANGUAGE.ENGLISH;
    [SerializeField]
    int accountId = 22;
    [SerializeField]
    string propertyName = "sid-multi-campaign.com";
    [SerializeField] 
    private CAMPAIGN_ENV campaignEnvironment = CAMPAIGN_ENV.PUBLIC;
    [SerializeField]
    long messageTimeoutInSeconds = 3;
    [SerializeField]
    string authID = null;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        List<SpCampaign> spCampaigns = new List<SpCampaign>();
        if (allCampaignTypesToLoad.Contains(CAMPAIGN_TYPE.GDPR))
        {
            List<TargetingParam> gdprParams = new List<TargetingParam> { new TargetingParam("location", "EU") };
            SpCampaign gdpr = new SpCampaign(CAMPAIGN_TYPE.GDPR, gdprParams);
            spCampaigns.Add(gdpr);
        }
        if (allCampaignTypesToLoad.Contains(CAMPAIGN_TYPE.CCPA))
        {
            List<TargetingParam> ccpaParams = new List<TargetingParam> { new TargetingParam("location", "US") };
            SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.CCPA, ccpaParams);
            spCampaigns.Add(ccpa);
        }
        if (allCampaignTypesToLoad.Contains(CAMPAIGN_TYPE.IOS14))
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                List<TargetingParam> ios14Params = new List<TargetingParam>();
                SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.IOS14, ios14Params);
                spCampaigns.Add(ccpa);
            }
            else
            {
                Debug.LogWarning("ios14 campaign is not allowed in non-ios device! Skipping it...");
            }
        }
        if (spCampaigns.Count > 0)
        {
            CMP.Initialize(spCampaigns: spCampaigns,
                           accountId: this.accountId,
                           propertyName: this.propertyName,
                           language: this.language,
                           campaignsEnvironment: campaignEnvironment,
                           messageTimeoutInSeconds: this.messageTimeoutInSeconds);
        }
        else
        {
            throw new System.Exception("You should add at least one SpCampaign to use CMP!");
        }
    }

    private void Start()
    {
        CMP.LoadMessage(authId: authID);
    }

    private void OnDestroy()
    {
        CMP.Dispose();
    }
}
