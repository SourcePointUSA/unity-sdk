using ConsentManagementProvider;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PrivacySettings : MonoBehaviour, IOnConsentReady, IOnConsentSpFinished
{
    public int accountId = 22;
    public int propertyId = 16893;    
    public string propertyName = "mobile.multicampaign.demo";
    public string gdprPmId = "488393";
    public string ccpaPmId = "509688";
    public string usnatPmId = "943886";
    public string authId = null;
    public List<CAMPAIGN_TYPE> campaignTypes = new ();

    [Header("GDPR Custom Consent")]
    public string[] vendors = { "5fbe6f050d88c7d28d765d47", "5ff4d000a228633ac048be41" };
    public string[] categories = { "60657acc9c97c400122f21f3", "608bad95d08d3112188e0e36", "608bad95d08d3112188e0e2f" };
    public string[] legIntCategories = { };

    [Header("UI")]
    public Text consentValueText;
    public Text authIdText;
    public Button loadMessageButton;
    public Button gdprPrivacySettingsButton;
    public Button ccpaPrivacySettingsButton;
    public Button usnatPrivacySettingsButton;
    public Button customConsentButton;
    public Button deleteCustomConsentButton;
    public Button clearDataButton;
    public Text sdkStatus;

    public static string statusCampaignGDPR = "default";
    public static string statusCampaignCCPA = "default";
    public static string statusCampaignUSNAT = "default";

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
    
    private string storedConsentString = null;
    
    private void Awake()
    {
        ConsentMessenger.AddListener<IOnConsentReady>(gameObject);
        ConsentMessenger.AddListener<IOnConsentSpFinished>(gameObject);

        List<SpCampaign> spCampaigns = new List<SpCampaign>();
        List<TargetingParam> gdprParams = new List<TargetingParam> { new TargetingParam("location", "EU") };
        SpCampaign gdpr = new SpCampaign(CAMPAIGN_TYPE.GDPR, gdprParams);
        spCampaigns.Add(gdpr);
        campaignTypes.Add(CAMPAIGN_TYPE.GDPR);
        
        List<TargetingParam> ccpaParams = new List<TargetingParam> { new TargetingParam("location", "US") };
        SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.CCPA, ccpaParams);
        spCampaigns.Add(ccpa);
        campaignTypes.Add(CAMPAIGN_TYPE.CCPA);

        List<TargetingParam> usnatParams = new List<TargetingParam> { new TargetingParam("location", "US") };
        SpCampaign usnat = new SpCampaign(CAMPAIGN_TYPE.USNAT, usnatParams, transitionCCPAAuth: false, supportLegacyUSPString: false);
        spCampaigns.Add(usnat);
        campaignTypes.Add(CAMPAIGN_TYPE.USNAT);

        UpdateSdkStatus("Running");

        CMP.Instance.Initialize(
            spCampaigns: spCampaigns,
            accountId: accountId,
            propertyId: propertyId,
            propertyName: propertyName,  // pay attention to any leading and trailing whitespaces; 
                                         // it's unlikely you voluntarily use them in property name, but if you do
                                         // please note that we Trim them down in the call tree.
            language: language,
            campaignsEnvironment: CAMPAIGN_ENV.PUBLIC,
            messageTimeoutInSeconds: 30
        );
    }

    void Start()
    {
        UpdateUI();
        CMP.Instance.LoadMessage(authId: authId);
    }

    public void OnLoadMessagePress()
    {
        UpdateSdkStatus("Running");
        storedConsentString = null;
        UpdateUI();
        CMP.Instance.LoadMessage(authId: authId);
    }

    public void OnGDPRPrivacyManagerButtonClick()
    {
        UpdateSdkStatus("Running");
        CMP.Instance.LoadPrivacyManager(
            campaignType: CAMPAIGN_TYPE.GDPR,
            pmId: gdprPmId
        );
    }

    public void OnCCPAPrivacyManagerButtonClick()
    {
        UpdateSdkStatus("Running");
        CMP.Instance.LoadPrivacyManager(
            campaignType: CAMPAIGN_TYPE.CCPA,
            pmId: ccpaPmId
        );
    }

    public void OnUSNATPrivacyManagerButtonClick()
    {
        UpdateSdkStatus("Running");
        CMP.Instance.LoadPrivacyManager(
            campaignType: CAMPAIGN_TYPE.USNAT,
            pmId: usnatPmId
        );
    }

    private void SuccessDelegate(GdprConsent customConsent)
    {
        Debug.Log($"I am your success callback!");
        CmpDebugUtil.ForceEnableNextCmpLog();
        CmpDebugUtil.Log(customConsent.ToFullString());
        storedConsentString = customConsent.euconsent;
        UpdateUI();
    }

    public void OnCustomConsentButtonClick()
    {
        CMP.Instance.CustomConsentGDPR(vendors: vendors,
            categories: categories,
            legIntCategories: legIntCategories,
            onSuccessDelegate: SuccessDelegate);
    }

    public void OnClearCustomConsentDataPress()
    {
        CMP.Instance.DeleteCustomConsentGDPR(
            vendors: vendors,
            categories: categories,
            legIntCategories: legIntCategories,
            onSuccessDelegate: SuccessDelegate
        );
        UpdateUI();
    }

    public void OnClearDataPress()
    {
        UpdateSdkStatus("Not Started");
        CMP.Instance.ClearAllData();
        storedConsentString = null;
        UpdateUI();
    }

    public void OnConsentReady(SpConsents consents)
    {
        storedConsentString = consents.gdpr.consents.euconsent ?? "--";
        if(CMP.Instance.UseGDPR) 
            CmpDebugUtil.Log(consents.gdpr.consents.ToFullString());
        if(CMP.Instance.UseCCPA) 
            CmpDebugUtil.Log(consents.ccpa.consents.ToFullString());
        if(CMP.Instance.UseUSNAT)
            CmpDebugUtil.Log(consents.usnat.consents.ToFullString());
        UpdateUI();
        statusCampaignGDPR =  UpdateStatuses(consents, CAMPAIGN_TYPE.GDPR);
        statusCampaignCCPA =  UpdateStatuses(consents, CAMPAIGN_TYPE.CCPA);
        statusCampaignUSNAT =  UpdateStatuses(consents, CAMPAIGN_TYPE.USNAT);
    }

    public void OnConsentSpFinished()
    {
        UpdateSdkStatus("Finished");
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (storedConsentString != null)
        {
            loadMessageButton.interactable = false;
            gdprPrivacySettingsButton.interactable = CMP.Instance.UseGDPR;
            ccpaPrivacySettingsButton.interactable = CMP.Instance.UseCCPA;
            usnatPrivacySettingsButton.interactable = CMP.Instance.UseUSNAT;
            customConsentButton.interactable = true;
            deleteCustomConsentButton.interactable = true;
            clearDataButton.interactable = true;
            consentValueText.text = storedConsentString;
            authIdText.text = CMP.GetBridgeString("AuthId:"+authId);
        }
        else
        {
            loadMessageButton.interactable = true;
            gdprPrivacySettingsButton.interactable = false;
            ccpaPrivacySettingsButton.interactable = false;
            usnatPrivacySettingsButton.interactable = false;
            customConsentButton.interactable = false;
            deleteCustomConsentButton.interactable = false;
            clearDataButton.interactable = false;
            consentValueText.text = "-";
            authIdText.text = "-";
        }
    }

    void UpdateSdkStatus(string status)
    {
        sdkStatus.text = "SDK:"+status;
    }

    string UpdateStatuses(SpConsents consents, CAMPAIGN_TYPE campaign)
    {
        if (campaign == CAMPAIGN_TYPE.GDPR)
        {
            if (consents.gdpr.consents.consentStatus == null)
                return "default";
            bool rejectedAny = (bool)consents.gdpr.consents.consentStatus.rejectedAny;
            bool rejectedLI = (bool)consents.gdpr.consents.consentStatus.rejectedLI;
            bool consentedAll = (bool)consents.gdpr.consents.consentStatus.consentedAll;
            bool consentedToAny = (bool)consents.gdpr.consents.consentStatus.consentedToAny;

            if (consentedAll && consentedToAny)
                return "accepted";
            else if (rejectedAny && rejectedLI)
                return "rejected";
        }
        if (campaign == CAMPAIGN_TYPE.CCPA)
        {
            if (consents.ccpa.consents.status == null)
                return "default";
            if (consents.ccpa.consents.status == "consentedAll")
                return "accepted";
            else if (consents.ccpa.consents.status == "rejectedAll")
                return "rejected";
        }
        if (campaign == CAMPAIGN_TYPE.USNAT)
        {
            if (consents.usnat.consents.statuses == null)
                return "default";
            bool rejectedAny = (bool)consents.usnat.consents.statuses.rejectedAny;
            bool consentedToAll = (bool)consents.usnat.consents.statuses.consentedToAll;
            bool consentedToAny = (bool)consents.usnat.consents.statuses.consentedToAny;

            if (consentedToAll && consentedToAny)
                return "accepted";
            else if (rejectedAny)
                return "rejected";            
        }
        return "default";
    }

    private void OnDestroy()
    {
        ConsentMessenger.RemoveListener<IOnConsentSpFinished>(gameObject);
        ConsentMessenger.RemoveListener<IOnConsentReady>(gameObject);
        CMP.Instance.Dispose();
    }
}
