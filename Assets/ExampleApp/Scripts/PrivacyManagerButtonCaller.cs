using ConsentManagementProvider;
using UnityEngine;

public class PrivacyManagerButtonCaller : MonoBehaviour
{
    [SerializeField]
    CAMPAIGN_TYPE campaignType;
    [Header("Only for Android:")]
    [SerializeField]
    PRIVACY_MANAGER_TAB privacyManagerTab = PRIVACY_MANAGER_TAB.DEFAULT;
    [SerializeField]
    string pmId;

    public void OnPrivacyManagerButtonClick()
    {
        CMP.Instance.LoadPrivacyManager(campaignType: this.campaignType,
                               pmId: this.pmId,
                               tab: this.privacyManagerTab);
    }
}
