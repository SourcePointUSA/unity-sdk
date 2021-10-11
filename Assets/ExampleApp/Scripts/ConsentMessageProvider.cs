using ConsentManagementProviderLib;
using System.Collections.Generic;
using UnityEngine;

public class ConsentMessageProvider : MonoBehaviour
{
    [SerializeField]
    List<CAMPAIGN_TYPE> allCampaignTypesToLoad;
    [SerializeField] 
    private GameObject CmpHomePrefab;
    [SerializeField] 
    private Canvas canvas;
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
            List<TargetingParam> ios14Params = new List<TargetingParam>();
            SpCampaign ios14 = new SpCampaign(CAMPAIGN_TYPE.IOS14, ios14Params);
            spCampaigns.Add(ios14);
        }
        CMP.Initialize(spCampaigns: spCampaigns,
                       accountId: this.accountId,
                       propertyName: this.propertyName,
                       language: this.language,
                       campaignsEnvironment: campaignEnvironment,
                       messageTimeoutInSeconds: this.messageTimeoutInSeconds);
    }

    private void Start()
    {
#if !UNITY_ANDROID || !UNITY_IOS || UNITY_EDITOR
        CMP.LoadMessage(cmpHomePrefab: CmpHomePrefab,
                        canvas: canvas,
                        privacyManagerId: "16879",
                        propertyId: "4933");
#else
        CMP.LoadMessage(authId: authID);
#endif
    }

    private void OnDestroy()
    {
        CMP.Dispose();
    }
}
