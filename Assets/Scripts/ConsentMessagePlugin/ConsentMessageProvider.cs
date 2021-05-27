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
    long messageTimeout = 3000;
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
        if (spCampaigns.Count > 0)
        {
            ConsentWrapperV6.Instance.InitializeLib(spCampaigns: spCampaigns,
                                                    accountId: this.accountId,
                                                    propertyName: this.propertyName,
                                                    language: this.language,
                                                    messageTimeout: this.messageTimeout);
        }
        else
        {
            throw new System.Exception("You should add at least one SpCampaign to use CMP!");
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            ConsentWrapperV6.Instance.LoadMessage(authId: authID);
        }
    }

    private void OnDestroy()
    {
        ConsentWrapperV6.Instance.Dispose();
    }
}
