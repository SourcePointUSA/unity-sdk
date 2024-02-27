using ConsentManagementProviderLib;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class PrivacySettings : MonoBehaviour, IOnConsentReady
{
    public int accountId = 22;
    public int propertyId = 16893;    
    public string propertyName = "mobile.multicampaign.demo";
    public bool useGDPR = true;
    public bool useCCPA = true;
    public string gdprPmId = "488393";
    public string ccpaPmId = "509688";
    public string authId = null;
    public List<CAMPAIGN_TYPE> campaignTypes = new ();

    // GDPR Custom Consent
    public string[] vendors = { "5fbe6f050d88c7d28d765d47", "5ff4d000a228633ac048be41" };
    public string[] categories = { "60657acc9c97c400122f21f3", "608bad95d08d3112188e0e36", "608bad95d08d3112188e0e2f" };
    public string[] legIntCategories = { };
    
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

    public Text consentValueText;
    public Button loadMessageButton;
    public Button gdprPrivacySettingsButton;
    public Button ccpaPrivacySettingsButton;
    public Button customConsentButton;
    public Button clearDataButton;

    private string storedConsentString = null;

    private void Awake()
    {
        ConsentMessenger.AddListener<IOnConsentReady>(gameObject);

        List<SpCampaign> spCampaigns = new List<SpCampaign>();
        if (useGDPR)
        {
            List<TargetingParam> gdprParams = new List<TargetingParam> { new TargetingParam("location", "EU") };
            SpCampaign gdpr = new SpCampaign(CAMPAIGN_TYPE.GDPR, gdprParams);
            spCampaigns.Add(gdpr);
        }
        if (useCCPA)
        {
            List<TargetingParam> ccpaParams = new List<TargetingParam> { new TargetingParam("location", "US") };
            SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.CCPA, ccpaParams);
            spCampaigns.Add(ccpa);
        }

        CMP.Initialize(
            spCampaigns: spCampaigns,
            accountId: accountId,
            propertyId: propertyId,
            propertyName: propertyName,
            gdpr: useGDPR, 
            ccpa: useCCPA, 
            language: language,
            gdprPmId: gdprPmId, 
            ccpaPmId: ccpaPmId,
            campaignsEnvironment: CAMPAIGN_ENV.PUBLIC,
            messageTimeoutInSeconds: 30
        );
    }

    void Start()
    {
        updateUI();
        CMP.LoadMessage(authId: authId);
    }

    public void OnGDPRPrivacyManagerButtonClick()
    {
        CMP.LoadPrivacyManager(
            campaignType: CAMPAIGN_TYPE.GDPR,
            pmId: gdprPmId,
            tab: PRIVACY_MANAGER_TAB.DEFAULT
        );
    }
    public void OnCustomConsentButtonClick()
    {
        CMP.CustomConsentGDPR(vendors: this.vendors,
            categories: this.categories,
            legIntCategories: this.legIntCategories,
            onSuccessDelegate: SuccessDelegate);
    }

    private void SuccessDelegate(GdprConsent customConsent)
    {
        Debug.Log($"I am your success callback!"); // TODO print customConsent
        storedConsentString = customConsent.euconsent;
        updateUI();
    }

    public void OnCCPAPrivacyManagerButtonClick()
    {
        CMP.LoadPrivacyManager(
            campaignType: CAMPAIGN_TYPE.CCPA,
            pmId: ccpaPmId,
            tab: PRIVACY_MANAGER_TAB.DEFAULT
        );
    }

    public void OnClearDataPress()
    {
        CMP.ClearAllData();
        storedConsentString = null;
        updateUI();
    }

    public void OnClearCustomConsentDataPress()
    {
        CMP.DeleteCustomConsentGDPR(
            vendors: this.vendors,
            categories: this.categories,
            legIntCategories: this.legIntCategories,
            onSuccessDelegate: SuccessDelegate
        );
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
            gdprPrivacySettingsButton.interactable = true;
            ccpaPrivacySettingsButton.interactable = true;
            customConsentButton.interactable = true;
            clearDataButton.interactable = true;
            consentValueText.text = storedConsentString;
        }
        else
        {
            loadMessageButton.interactable = true;
            gdprPrivacySettingsButton.interactable = false;
            ccpaPrivacySettingsButton.interactable = false;
            customConsentButton.interactable = false;
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
