using System;
using System.Collections.Generic;
using ConsentManagementProviderLib.Enum;
using ConsentManagementProviderLib.Observer;
using UnityEngine;

namespace ConsentManagementProviderLib.Android
{
    internal class ConsentWrapperAndroid
    {
        private AndroidJavaObject consentLib;
        private AndroidJavaObject activity;
        private SpClientProxy spClient;

        private AndroidJavaConstruct constructor;
        private CustomConsentClient customConsentClient;
        
        private static ConsentWrapperAndroid instance;
        public static ConsentWrapperAndroid Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConsentWrapperAndroid();
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        ConsentWrapperAndroid()
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

        public void InitializeLib(List<SpCampaign> spCampaigns, int accountId, int propertyId, string propertyName, MESSAGE_LANGUAGE language, CAMPAIGN_ENV campaignsEnvironment, long messageTimeoutMilliSeconds = 3000)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
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
                        AndroidJavaObject campaign = constructor.ConstructCampaign(typeAJO, paramsList, sp.CampaignType);
                        campaigns[spCampaigns.IndexOf(sp)] = campaign;
                    }
                    AndroidJavaObject spConfig = constructor.ConstructSpConfig(accountId: accountId,
                                                                               propertyId: propertyId,
                                                                               propertyName: propertyName,
                                                                               messageTimeout: messageTimeoutMilliSeconds,
                                                                               language: msgLang,
                                                                               campaignsEnvironment: campaignsEnvironment,
                                                                               spCampaigns: campaigns);
                    consentLib = constructor.ConsrtuctLib(spConfig: spConfig,
                                                          activity: this.activity,
                                                          spClient: this.spClient);
                }
                catch (Exception e)
                {
                    CmpDebugUtil.LogError(e.Message);
                }
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

        public GdprConsent GetCustomGdprConsent()
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
        
        internal void Dispose()
        {
            if (consentLib != null)
            {
                constructor.Dispose();
                CmpDebugUtil.Log("Disposing consentLib...");
                consentLib.Call("dispose");
                CmpDebugUtil.Log("Disposing consentLib successfully done");
            }
        }

        internal void CallShowView(AndroidJavaObject view)
        {
            consentLib.Call("showView", view);
            CmpDebugUtil.Log("C# : View showing passed to Android's consent lib");
        }

        internal void CallRemoveView(AndroidJavaObject view)
        {
            consentLib.Call("removeView", view);
            CmpDebugUtil.Log("C# : View removal passed to Android's consent lib");
        }

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
    }
}