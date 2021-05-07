using GdprConsentLib;
using UnityEngine;

public class ConsentButtonCaller : MonoBehaviour
{
    [SerializeField]
    CAMPAIGN_TYPE campaign = CAMPAIGN_TYPE.GDPR;
    [SerializeField]
    PRIVACY_MANAGER_TAB privacyManagerTab = PRIVACY_MANAGER_TAB.DEFAULT;
    [SerializeField]
    MESSAGE_LANGUAGE language = MESSAGE_LANGUAGE.ENGLISH;
    [SerializeField]
    int accountId = 22;
    [SerializeField]
    string propertyName = "http://sid-multi-campaign.com";
    [SerializeField]
    string pmId = "404472";
    [SerializeField]
    string authID = null;

    private void Awake()
    {
        ConsentWrapperV6.Instance.InitializeLib(this.campaign,
                                                this.accountId,
                                                this.propertyName,
                                                this.privacyManagerTab,
                                                this.language);
    }

    private void Start()
    {
        ConsentWrapperV6.Instance.LoadMessage(this.campaign, authID);
    }

    public void OnConsentButtonClick()
    {
        ConsentWrapperV6.Instance.CallConsentAAR(this.campaign,
                                                 this.accountId,
                                                 this.propertyName,
                                                 this.pmId,
                                                 this.privacyManagerTab,
                                                 this.language);
    }
}
