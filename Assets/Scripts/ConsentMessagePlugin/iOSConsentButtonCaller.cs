using System;
using System.Runtime.InteropServices;
using GdprConsentLib;
using UnityEngine;

public class iOSConsentButtonCaller : MonoBehaviour
{
    [SerializeField]
    PRIVACY_MANAGER_TAB privacyManagerTab = PRIVACY_MANAGER_TAB.DEFAULT;
    [SerializeField]
    int accountId = 22;
    [SerializeField]
    int propertyId = 7639;
    [SerializeField]
    string propertyName = "tcfv2.mobile.webview";
    [SerializeField]
    string pmId = "122058";
    [SerializeField]
    string authID = null;

    [DllImport("__Internal")]
    private static extern void _initializeSDKTest(); 
    [DllImport("__Internal")]
    private static extern void _loadMessage(int accountId, int propertyId, string propertyName, string pmId);

    public void OnConsentButtonClick()
    {
        /* TODO:
           campaignEnv: GDPRCampaignEnvPublic
             GDPRCampaignEnvStage = 0,
             GDPRCampaignEnvPublic = 1,
           targetingParams:targetingParameter
         */
        _loadMessage(accountId,
                    propertyId,
                    propertyName,
                    pmId);
        // _initializeSDKTest();
    }
}
