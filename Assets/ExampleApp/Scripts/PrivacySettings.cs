using ConsentManagementProviderLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class PrivacySettings : MonoBehaviour, IOnConsentReady
{
    public int accountId = 1909;
    public string propertyName = "voodoo.ios";
    public int propertyId = 31817;
    public string pmId = "838714";
    public string authId = null;
    public List<CAMPAIGN_TYPE> campaignTypes = new ();

    private List<SpCampaign> Campaigns
    {
        get
        {
            return campaignTypes.ConvertAll(type => new SpCampaign(type, targetingParams: new List<TargetingParam>()));
        }
    }

    private MESSAGE_LANGUAGE language
    {
        get
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Spanish: return MESSAGE_LANGUAGE.SPANISH;
                case SystemLanguage.French: return MESSAGE_LANGUAGE.FRENCH;
                default: return MESSAGE_LANGUAGE.ENGLISH;
            }
        }
    }

    public TextMeshProUGUI consentValueText;
    public Button loadMessageButton;
    public Button privacySettingsButton;
    public Button clearDataButton;

    private string storedConsentString = null;

    private void Awake()
    {
        ConsentMessenger.AddListener<IOnConsentReady>(gameObject);

        CMP.Initialize(
            spCampaigns: Campaigns,
            accountId: accountId,
            propertyId: propertyId,
            propertyName: propertyName,
            language: language,
            campaignsEnvironment: CAMPAIGN_ENV.PUBLIC,
            messageTimeoutInSeconds: 30
        );
    }

    void Start()
    {
        updateUI();
        CMP.LoadMessage(authId: authId);
    }

    public void OnPrivacyManagerButtonClick()
    {
        CMP.LoadPrivacyManager(
            campaignType: CAMPAIGN_TYPE.GDPR,
            pmId: pmId,
            tab: PRIVACY_MANAGER_TAB.DEFAULT
        );
    }

    public void OnClearDataPress()
    {
        PlayerPrefs.DeleteAll();
        storedConsentString = null;
        updateUI();
    }

    public void OnLoadMessagePress()
    {
        storedConsentString = null;
        updateUI();
        CMP.LoadMessage(authId: authId);
    }

    public void OnConsentReady(SpConsents consents)
    {
        storedConsentString = consents.gdpr.consents.euconsent;
        updateUI();
    }

    private void updateUI()
    {
        if (storedConsentString != null)
        {
            loadMessageButton.interactable = false;
            privacySettingsButton.interactable = true;
            clearDataButton.interactable = true;
            consentValueText.text = storedConsentString;
        }
        else
        {
            loadMessageButton.interactable = true;
            privacySettingsButton.interactable = false;
            clearDataButton.interactable = false;
            consentValueText.text = "-";
        }
    }

    private void OnDestroy()
    {
        ConsentMessenger.RemoveListener<IOnConsentReady>(gameObject);
        CMP.Dispose();
    }
}
