using GdprConsentLib;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConsentWrapperV6
{
    private AndroidJavaObject consentLib;
    AndroidJavaObject activity;
    SpClientProxy spClient;

    AndroidJavaConstruct constructor;

    private static ConsentWrapperV6 instance;
    public static ConsentWrapperV6 Instance
    {
        get
        {
            if (instance == null)
                instance = new ConsentWrapperV6();
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    ConsentWrapperV6()
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            activity = AndroidJavaConstruct.GetActivity();
            DebugUtil.Log("Activity is OK");
            spClient = new SpClientProxy();
            DebugUtil.Log("spClient is OK");
            this.constructor = new AndroidJavaConstruct();
            DebugUtil.Log("AndroidJavaConstruct obj is OK");
        }
#endif
    }

    public void InitializeLib(List<CAMPAIGN_TYPE> spCampaigns, int accountId, string propertyName, MESSAGE_LANGUAGE language)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                AndroidJavaObject msgLang = constructor.ConstructMessageLanguage(language);
                AndroidJavaObject[] campaigns = new AndroidJavaObject[spCampaigns.Count];
                foreach (CAMPAIGN_TYPE type in spCampaigns)
                {
                    AndroidJavaObject typeAJO = constructor.ConstructCampaignType(type);
                    AndroidJavaObject campaign = constructor.ConstructCampaign(typeAJO, type);
                    campaigns[spCampaigns.IndexOf(type)] = campaign;
                }
                consentLib = constructor.ConsrtuctLib(campaigns, accountId, propertyName, msgLang, this.activity, this.spClient);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.Message);
            }
        }
#endif
    }

    public void LoadMessage(string authID = null)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                if (string.IsNullOrEmpty(authID))
                {
                    RunOnUiThread(delegate { InvokeLoadMessage(); });
                }
                else
                {
                    //TODO: check InvokeLoadMessageWithAuthID
                    RunOnUiThread(delegate { InvokeLoadMessageWithAuthID(authID); });
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.Message);
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
                DebugUtil.LogError(e.Message);
            }
        }
#endif
    }

    public void CustomConsentGDPR(string[] vendors, string[] categories, string[] legIntCategories, Action<string> onSuccessDelegate)
    {
        consentLib.Call("customConsentGDPR", vendors, categories, legIntCategories, new CustomConsentClient(onSuccessDelegate));
    }

    internal void Dispose()
    {
        if(consentLib!=null)
        {
            constructor.Dispose();
            DebugUtil.Log("Disposing consentLib...");
            consentLib.Call("dispose");
            DebugUtil.Log("Disposing consentLib successfully done");
        }
    }

    internal void CallShowView(AndroidJavaObject view)
    {
        consentLib.Call("showView", view);
        DebugUtil.Log("C# : View showing passed to Android's consent lib");
    }

    internal void CallRemoveView(AndroidJavaObject view)
    {
        consentLib.Call("removeView", view);
        DebugUtil.Log("C# : View removal passed to Android's consent lib");
    }

    private void RunOnUiThread(Action action)
    {
        DebugUtil.Log(">>>STARTING RUNNABLE ON UI THREAD!");
        activity.Call("runOnUiThread", new AndroidJavaRunnable(action));
    }

    private void InvokeLoadMessage()
    {
        DebugUtil.Log("InvokeLoadMessage() STARTING...");
        try
        {
            consentLib.Call("loadMessage");
            DebugUtil.Log($"loadMessage() is OK...");
        }
        catch (Exception ex) { DebugUtil.LogError(ex.Message); }
        finally { DebugUtil.Log($"InvokeLoadMessage() DONE"); }
    }

    private void InvokeLoadPrivacyManager(string pmId, AndroidJavaObject tab, AndroidJavaObject campaignType, CAMPAIGN_TYPE campaignTypeForLog)
    {
        DebugUtil.Log("InvokeLoadPrivacyManager() STARTING...");
        try
        {
            consentLib.Call("loadPrivacyManager", pmId, tab, campaignType);
            DebugUtil.Log($"loadPrivacyManager() with {campaignTypeForLog} is OK...");
        }
        catch (Exception ex) { DebugUtil.LogError(ex.Message); }
        finally { DebugUtil.Log($"InvokeLoadPrivacyManager() with {campaignTypeForLog} DONE"); }
    }

    private void InvokeLoadMessageWithAuthID(string authID)
    {
        DebugUtil.Log("loadMessage(authId: String) STARTING...");
        try
        {
            consentLib.Call("loadMessage", authID);
        }
        catch (Exception ex) { DebugUtil.LogError(ex.Message); }
        finally { DebugUtil.Log("loadMessage(authId: String) DONE"); }
    }
}