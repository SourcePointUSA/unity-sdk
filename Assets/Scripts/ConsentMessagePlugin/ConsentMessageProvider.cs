using GdprConsentLib;
using System.Collections.Generic;
using UnityEngine;

public class ConsentMessageProvider : MonoBehaviour
{
    [SerializeField]
    List<SpCampaignScriptableObject> allSpCampaignsToLoad;
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
        ConsentWrapperV6.Instance.InitializeLib(this.allSpCampaignsToLoad,
                                                this.accountId,
                                                this.propertyName,
                                                this.language,
                                                this.messageTimeout);
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
