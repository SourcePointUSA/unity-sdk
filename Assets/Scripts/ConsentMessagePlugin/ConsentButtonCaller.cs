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
        CMP.LoadPrivacyManager(campaignType: this.campaignType,
                               pmId: this.pmId,
                               tab: this.privacyManagerTab);
    }
}
