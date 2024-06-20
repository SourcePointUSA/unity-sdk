using System;
using System.Linq;
using System.Collections.Generic;
using ConsentManagementProviderLib.Enum;
using ConsentManagementProviderLib.Observer;
using UnityEngine;

namespace ConsentManagementProviderLib.Android
{
    internal class ConsentWrapperAndroid: ISpSdkAndroid
    {
        private AndroidJavaObject consentLib;
        private AndroidJavaObject activity;
        private SpClientProxy spClient;

        private AndroidJavaConstruct constructor;
        private CustomConsentClient customConsentClient;

        public ConsentWrapperAndroid()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                activity = AndroidJavaConstruct.GetActivity();
                CmpDebugUtil.Log("Activity is OK");
                spClient = new SpClientProxy();
                CmpDebugUtil.Log("spClient is OK");
                this.constructor = new AndroidJavaConstruct();
                CmpDebugUtil.Log("AndroidJavaConstruct obj is OK");
            }
#endif
        }

        public void Initialize(
            int accountId, 
            int propertyId, 
            string propertyName, 
            MESSAGE_LANGUAGE language, 
            string gdprPmId, 
            string ccpaPmId, 
            string usnatPmId, 
            List<SpCampaign> spCampaigns, 
            CAMPAIGN_ENV campaignsEnvironment, 
            long messageTimeoutInSeconds = 3, 
            bool? transitionCCPAAuth = null, 
            bool? supportLegacyUSPString = null)
        {
            if (!ValidateSpCampaigns(ref spCampaigns))
            {
                return;
            }
            long messageTimeoutMilliSeconds = messageTimeoutInSeconds * 1000;
#if UNITY_ANDROID
            try
                {
                    AndroidJavaObject msgLang = constructor.ConstructMessageLanguage(language);
                    AndroidJavaObject[] campaigns = new AndroidJavaObject[spCampaigns.Count];
                    foreach (SpCampaign sp in spCampaigns)
                    {
                        AndroidJavaObject typeAJO = constructor.ConstructCampaignType(sp.CampaignType);
                        AndroidJavaObject[] paramsArray = new AndroidJavaObject[sp.TargetingParams.Count];
                        foreach (TargetingParam tp in sp.TargetingParams)
                        {
                            AndroidJavaObject param = constructor.ConstructTargetingParam(tp.Key, tp.Value);
                            paramsArray[sp.TargetingParams.IndexOf(tp)] = param;
                        }
                        AndroidJavaObject paramsList = CmpJavaToUnityUtils.ConvertArrayToList(paramsArray);
                        AndroidJavaObject campaign;
                        if (sp.CampaignType == CAMPAIGN_TYPE.USNAT && (transitionCCPAAuth.HasValue || supportLegacyUSPString.HasValue))
                            campaign = constructor.ConstructCampaign(typeAJO, paramsList, sp.CampaignType, transitionCCPAAuth, supportLegacyUSPString);
                        else
                            campaign = constructor.ConstructCampaign(typeAJO, paramsList, sp.CampaignType);
                        campaigns[spCampaigns.IndexOf(sp)] = campaign;
                    }
                    AndroidJavaObject spConfig = constructor.ConstructSpConfig(accountId: accountId,
                                                                               propertyId: propertyId,
                                                                               propertyName: propertyName,
                                                                               messageTimeout: messageTimeoutMilliSeconds,
                                                                               language: msgLang,
                                                                               campaignsEnvironment: campaignsEnvironment,
                                                                               spCampaigns: campaigns);
                    consentLib = constructor.ConstructLib(spConfig: spConfig,
                                                          activity: this.activity,
                                                          spClient: this.spClient);
                }
                catch (Exception e)
                {
                    CmpDebugUtil.LogError(e.Message);
                }
#endif
        }

        public void LoadMessage(string authId = null)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    if (string.IsNullOrEmpty(authId))
                    {
                        RunOnUiThread(delegate { InvokeLoadMessage(); });
                    }
                    else
                    {
                        RunOnUiThread(delegate { InvokeLoadMessageWithAuthID(authId); });
                    }
                }
                catch (Exception e)
                {
                    CmpDebugUtil.LogError(e.Message);
                }
            }
#endif
        }

        public void LoadPrivacyManager(CAMPAIGN_TYPE campaignType, string pmId, PRIVACY_MANAGER_TAB tab)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    AndroidJavaObject type = constructor.ConstructCampaignType(campaignType);
                    AndroidJavaObject privacyManagerTab = constructor.ConstructPrivacyManagerTab(tab);
                    RunOnUiThread(delegate { InvokeLoadPrivacyManager(pmId, privacyManagerTab, type, campaignType); });
                }
                catch (Exception e)
                {
                    CmpDebugUtil.LogError(e.Message);
                }
            }
#endif
        }

        public void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate)
        {
            this.customConsentClient = new CustomConsentClient(onSuccessDelegate);
            consentLib.Call("customConsentGDPR", vendors, categories, legIntCategories, customConsentClient);
        }

        public void DeleteCustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<GdprConsent> onSuccessDelegate)
        {
            this.customConsentClient = new CustomConsentClient(onSuccessDelegate);
            consentLib.Call("deleteCustomConsentTo", vendors, categories, legIntCategories, customConsentClient);
        }

        public SpConsents GetSpConsents()
        {
            if (spClient != null)
            {
                return spClient._spConsents;
            }
            else
            {
                return null;
            }
        }

        public GdprConsent GetCustomConsent()
        {
            if (this.customConsentClient != null)
            {
                return customConsentClient.customGdprConsent;
            }
            else
            {
                return null;
            }
        }

        public void ClearAllData()
        {
            SpAndroidNativeUtils.ClearAllData();
        }
        
        public void Dispose()
        {
            if (consentLib != null)
            {
                constructor.Dispose();
                CmpDebugUtil.Log("Disposing consentLib...");
                consentLib.Call("dispose");
                CmpDebugUtil.Log("Disposing consentLib successfully done");
            }
        }

        public void CallShowView(AndroidJavaObject view)
        {
            consentLib.Call("showView", view);
            CmpDebugUtil.Log("C# : View showing passed to Android's consent lib");
        }

        public void CallRemoveView(AndroidJavaObject view)
        {
            consentLib.Call("removeView", view);
            CmpDebugUtil.Log("C# : View removal passed to Android's consent lib");
        }

        #region private methods
        private void RunOnUiThread(Action action)
        {
            CmpDebugUtil.Log(">>>STARTING RUNNABLE ON UI THREAD!");
            activity.Call("runOnUiThread", new AndroidJavaRunnable(action));
        }

        private void InvokeLoadMessage()
        {
            CmpDebugUtil.Log("InvokeLoadMessage() STARTING...");
            try
            {
                consentLib.Call("loadMessage");
                CmpDebugUtil.Log($"loadMessage() is OK...");
            }
            catch (Exception ex) { CmpDebugUtil.LogError(ex.Message); }
            finally { CmpDebugUtil.Log($"InvokeLoadMessage() DONE"); }
        }

        private void InvokeLoadPrivacyManager(string pmId, AndroidJavaObject tab, AndroidJavaObject campaignType, CAMPAIGN_TYPE campaignTypeForLog)
        {
            CmpDebugUtil.Log("InvokeLoadPrivacyManager() STARTING...");
            try
            {
                consentLib.Call("loadPrivacyManager", pmId, tab, campaignType);
                CmpDebugUtil.Log($"loadPrivacyManager() with {campaignTypeForLog} is OK...");
            }
            catch (Exception ex) { CmpDebugUtil.LogError(ex.Message); }
            finally { CmpDebugUtil.Log($"InvokeLoadPrivacyManager() with {campaignTypeForLog} DONE"); }
        }

        private void InvokeLoadMessageWithAuthID(string authID)
        {
            CmpDebugUtil.Log("loadMessage(authId: String) STARTING...");
            try
            {
                consentLib.Call("loadMessage", authID);
            }
            catch (Exception ex) { CmpDebugUtil.LogError(ex.Message); }
            finally { CmpDebugUtil.Log("loadMessage(authId: String) DONE"); }
        }

        private static bool ValidateSpCampaigns(ref List<SpCampaign> spCampaigns)
        {
            List<SpCampaign> ios14 = spCampaigns.Where(campaign => campaign.CampaignType == CAMPAIGN_TYPE.IOS14).ToList();
            if (ios14 != null && ios14.Count > 0)
            {
                Debug.LogWarning("ios14 campaign is not allowed in non-ios device! Skipping it...");
                foreach (SpCampaign ios in ios14)
                {
                    spCampaigns.Remove(ios);
                }
            }
            if (spCampaigns.Count == 0)
            {
                Debug.LogError("You should add at least one SpCampaign to use CMP! Aborting...");
                return false;
            }
            return true;
        }
        #endregion
    }
}