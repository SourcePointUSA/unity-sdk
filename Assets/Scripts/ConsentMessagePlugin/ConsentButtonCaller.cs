using ConsentManagementProviderLib;
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
}
