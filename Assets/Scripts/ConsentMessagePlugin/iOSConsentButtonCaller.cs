using System;
using System.Runtime.InteropServices;
using ConsentManagementProviderLib;
using UnityEngine;

public class iOSConsentButtonCaller : MonoBehaviour
{
    [SerializeField]
    PRIVACY_MANAGER_TAB privacyManagerTab = PRIVACY_MANAGER_TAB.DEFAULT;
    [SerializeField]
    int accountId = 22;
    [SerializeField]
    string propertyName = "mobile.multicampaign.demo";
    [SerializeField]
    string authID = null;
    
    [DllImport("__Internal")]
    private static extern void _loadMessage(string authId);
    [DllImport("__Internal")]
    private static extern void _addTargetingParamForCampaignType(int campaignType, string key, string value);
    [DllImport("__Internal")]
    private static extern void _consrtuctLib(int accountId, string propName, int arrSize, int[] campaignTypes, int[] campaignEnvironments, long timeOutSeconds);
    [DllImport("__Internal")]
    private static extern void _setMessageLanguage(int langId);
    [DllImport("__Internal")]
    private static extern void _loadGDPRPrivacyManager(string pmId, int tabId);
    [DllImport("__Internal")]
    private static extern void _loadCCPAPrivacyManager(string pmId, int tabId);

    [DllImport("__Internal")]
    private static extern void _cleanDict();
    [DllImport("__Internal")]
    private static extern void _cleanArrays();
    [DllImport("__Internal")]
    private static extern void _customConsentGDPRWithVendors();
    [DllImport("__Internal")]
    private static extern void _addVendor(string vendor);
    [DllImport("__Internal")]
    private static extern void _addLegIntCategory(string legIntCategory);
    [DllImport("__Internal")]
    private static extern void _addCategory(string category);

    [SerializeField] 
    private bool ONLY = false;
    
/*
  typedef SWIFT_ENUM(NSInteger, SPCampaignEnv, open) {
  SPCampaignEnvStage = 0,
  SPCampaignEnvPublic = 1,
}; 
 */

    private void Awake()
    {
        if (ONLY)
        {
            _cleanDict();
            // _addTargetingParamForCampaignType(0, "key0", "value0");
            _consrtuctLib(accountId: accountId,
                propName: propertyName,
                arrSize: 3,
                campaignTypes: new[] {0, 1, 2},
                campaignEnvironments: new[] {1, 1, 1},
                timeOutSeconds: 3);
            // _setMessageLanguage();
        }
    }

    private void Start()
    {
        if (ONLY)
        {
            _loadMessage(authID);
        }
    }

    public void OnGDPRConsentButtonClick()
    {
        _loadGDPRPrivacyManager("488393", 3);
    }

    public void OnCCPAConsentButtonClick()
    {
        _loadCCPAPrivacyManager("14967", 1);
    }
    
    public void OnCustomConsentButtonClick()
    {
        _cleanArrays();
        _addVendor("5fbe6f050d88c7d28d765d47");
        _addCategory("60657acc9c97c400122f21f3");
        _customConsentGDPRWithVendors();
    }
}
