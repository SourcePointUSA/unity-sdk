using ConsentManagementProviderLib;
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
    private CMP cmp = CMP.GetInstance();

    public void OnPrivacyManagerButtonClick()
    {
        cmp.LoadPrivacyManager(campaignType: this.campaignType,
                               pmId: this.pmId,
                               tab: this.privacyManagerTab);
    }
}
