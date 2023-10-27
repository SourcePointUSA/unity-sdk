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
    int propertyId = 16893;
    [SerializeField]
    string propertyName = "mobile.multicampaign.demo";
    [SerializeField] 
    private CAMPAIGN_ENV campaignEnvironment = CAMPAIGN_ENV.PUBLIC;
    [SerializeField]
    long messageTimeoutInSeconds = 30;
    [SerializeField]
    string authID = null;
    [Header("Only for IOS:")]
    [SerializeField] 
    bool gdpr = true;
    [SerializeField] 
    bool ccpa = true;
    [SerializeField] 
    string gdprPmId = "488393";
    [SerializeField] 
    string ccpaPmId = "509688";

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
        CmpDebugUtil.Log("propertyId IN ConsentMessageProvider " + propertyId.ToString());
        CmpDebugUtil.Log("accountId IN ConsentMessageProvider " + accountId.ToString());
        CmpDebugUtil.Log("propertyName IN ConsentMessageProvider " + propertyName);
        CMP.Initialize(spCampaigns: spCampaigns,
                       accountId: this.accountId,
                       propertyId: this.propertyId,
                       propertyName: this.propertyName,
                       gdpr: this.gdpr,
                       ccpa: this.ccpa,
                       language: this.language,
                       gdprPmId: this.gdprPmId,
                       ccpaPmId: this.ccpaPmId,
                       campaignsEnvironment: campaignEnvironment,
                       messageTimeoutInSeconds: this.messageTimeoutInSeconds);
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
