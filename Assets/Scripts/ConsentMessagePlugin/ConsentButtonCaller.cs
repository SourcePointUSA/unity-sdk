using GdprConsentLib;
using UnityEngine;

public class ConsentButtonCaller : MonoBehaviour
{
    [SerializeField]
    CAMPAIGN_TYPE campaignType;
    [SerializeField]
    PRIVACY_MANAGER_TAB privacyManagerTab = PRIVACY_MANAGER_TAB.DEFAULT;
    [SerializeField]
    string pmId;

    public void OnConsentButtonClick()
    {
        ConsentWrapperV6.Instance.LoadPrivacyManager(this.campaignType,
                                                     this.pmId,
                                                     this.privacyManagerTab);
    }

    public void OnCustomConsentButtonClick()
    {
        ConsentWrapperV6.Instance.CustomConsentGDPR(new string[] { "5fbe6f050d88c7d28d765d47" },
                                                    new string[] { "60657acc9c97c400122f21f3" },
                                                    new string[] { },
                                                    delegate (string spConsentsJson) { Debug.Log($"I am your callback! {spConsentsJson}"); });
    }
}
