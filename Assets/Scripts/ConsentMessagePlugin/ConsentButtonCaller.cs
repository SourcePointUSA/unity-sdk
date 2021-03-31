using GdprConsentLib;
using UnityEngine;

public class ConsentButtonCaller : MonoBehaviour
{
    [SerializeField]
    PRIVACY_MANAGER_TAB privacyManagerTab = PRIVACY_MANAGER_TAB.DEFAULT;
    [SerializeField]
    int accountId = 22;
    [SerializeField]
    int propertyId = 10589;
    [SerializeField]
    string propertyName = "http://sid-multi-campaign.com";
    [SerializeField]
    string pmId = "404472";
    [SerializeField]
    string authID = null;

    public void OnConsentButtonClick()
    {
        if (string.IsNullOrEmpty(authID))
        {
            ConsentWrapperV6.Instance.CallConsentAAR(this.accountId,
                                                     this.propertyId,
                                                     this.propertyName,
                                                     this.pmId,
                                                     this.privacyManagerTab);
        }
        else
        {
            ConsentWrapperV6.Instance.CallConsentAAR(this.accountId,
                                                     this.propertyId,
                                                     this.propertyName,
                                                     this.pmId,
                                                     this.privacyManagerTab,
                                                     this.authID);
        }
    }
}
