using ConsentManagementProviderLib;
using UnityEngine;

public class PrivacyManagerButtonCaller : MonoBehaviour
{
    [SerializeField]
    CAMPAIGN_TYPE campaignType;
    [SerializeField]
    PRIVACY_MANAGER_TAB privacyManagerTab = PRIVACY_MANAGER_TAB.DEFAULT;
    [SerializeField]
    string pmId;

    public void OnPrivacyManagerButtonClick()
    {
        CMP.LoadPrivacyManager(campaignType: this.campaignType,
                               privacyManagerId: this.pmId,
                               tab: this.privacyManagerTab);
    }
}
