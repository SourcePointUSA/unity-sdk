using GdprConsentLib;
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
    string authID = null;

    private void Awake()
    {
        ConsentWrapperV6.Instance.InitializeLib(this.allCampaignTypesToLoad,
                                                this.accountId,
                                                this.propertyName,
                                                this.language);
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            ConsentWrapperV6.Instance.LoadMessage(authID);
        }
    }

    private void OnDestroy()
    {
        ConsentWrapperV6.Instance.Dispose();
    }
}
