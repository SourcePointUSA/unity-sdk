using GdprConsentLib;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ConsentWrapperV6
{
    static readonly string androidPluginName = "com.sourcepoint.cmplibrary.creation.FactoryKt";
    AndroidJavaClass pluginBuilderClass;

    private AndroidJavaObject consentLib;
    AndroidJavaObject activity;
    SpClientProxy spClient;

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
            pluginBuilderClass = new AndroidJavaClass(androidPluginName);
            Util.Log("plugin class is OK");
            activity = GetActivity();
            Util.Log("Activity is OK");
            spClient = new SpClientProxy();
            Util.Log("spClient is OK");
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
                AndroidJavaObject msgLang = ConstructMessageLanguage(language);
                AndroidJavaObject[] campaigns = new AndroidJavaObject[spCampaigns.Count];
                foreach (CAMPAIGN_TYPE type in spCampaigns)
                {
                    AndroidJavaObject typeAJO = ConstructCampaignType(type);
                    AndroidJavaObject campaign = ConstructCampaign(typeAJO);
                    campaigns[spCampaigns.IndexOf(type)] = campaign;
                }
                consentLib = ConsrtuctLib(campaigns, accountId, propertyName, msgLang);
            }
            catch (Exception e)
            {
                Util.LogError(e.Message);
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
                Util.LogError(e.Message);
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
                AndroidJavaObject type = ConstructCampaignType(campaignType);
                AndroidJavaObject privacyManagerTab = ConstructPrivacyManagerTab(tab);
                RunOnUiThread(delegate { InvokeLoadPrivacyManager(pmId, privacyManagerTab, type, campaignType); });
            }
            catch (Exception e)
            {
                Util.LogError(e.Message);
            }
        }
#endif
    }

    internal void Dispose()
    {
        if(consentLib!=null)
        {
            Util.Log("Disposing consentLib...");
            consentLib.Call("dispose");
        }
    }

    internal void CallShowView(AndroidJavaObject view)
    {
        consentLib.Call("showView", view);
        Util.Log("C# : View showing passed to Android's consent lib");
    }

    internal void CallRemoveView(AndroidJavaObject view)
    {
        consentLib.Call("removeView", view);
        Util.Log("C# : View removal passed to Android's consent lib");
    }

    private AndroidJavaObject ConsrtuctLib(AndroidJavaObject[] campaigns, int accountId, string propertyName, AndroidJavaObject language)
    {
        AndroidJavaObject spConfig = ConstructSpConfig(accountId, propertyName, language, campaigns);
        AndroidJavaObject lib = pluginBuilderClass.CallStatic<AndroidJavaObject>("makeConsentLib", spConfig, activity, spClient);
        Util.Log("consentLib is OK");
        return lib;
    }

    private AndroidJavaObject ConstructSpConfig(int accountId, string propertyName, AndroidJavaObject language, AndroidJavaObject[] spCampaigns)
    {
        AndroidJavaObject spConfig;
        using (AndroidJavaObject SpConfigDataBuilderClass = new AndroidJavaObject("com.sourcepoint.cmplibrary.creation.SpConfigDataBuilder"))
        {
            SpConfigDataBuilderClass.Call<AndroidJavaObject>("addAccountId", accountId);
            Util.Log("addAccountId is OK");
            SpConfigDataBuilderClass.Call<AndroidJavaObject>("addPropertyName", propertyName);
            Util.Log("addPropertyName is OK");
            SpConfigDataBuilderClass.Call<AndroidJavaObject>("addMessageLanguage", language);
            Util.Log("addMessageLanguage is OK");

            foreach (AndroidJavaObject camp in spCampaigns)
            {
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addCampaign", camp);
                Util.Log("addCampaign is OK");
            }

            spConfig = SpConfigDataBuilderClass.Call<AndroidJavaObject>("build");
            Util.Log("build() is OK");
        }
        Util.Log("SpConfig is OK");
        return spConfig;
    }
    
    private AndroidJavaObject ConstructPrivacyManagerTab(PRIVACY_MANAGER_TAB tab)
    {
        AndroidJavaObject privacyManagerTabK = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.PMTab");
        privacyManagerTabK.Set("key", CSharp2JavaStringEnumMapper.GetPrivacyManagerTabKey(tab));
        Util.Log("PMTab is OK");
        return privacyManagerTabK;
    }

    private AndroidJavaObject ConstructCampaign(AndroidJavaObject campaignType)
    {
        AndroidJavaObject param = ConstructTargetingParam("location", "EU");
        AndroidJavaObject paramList = ConvertArrayToList(new AndroidJavaObject[] { param });
        AndroidJavaObject campaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", campaignType, paramList);
        Util.Log($"Campaign {campaignType} is OK");
        return campaign;
    }

    private AndroidJavaObject ConstructCampaignType(CAMPAIGN_TYPE campaignType)
    {
        AndroidJavaObject type = null;
        switch (campaignType)
        {
            case CAMPAIGN_TYPE.GDPR:
                type = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.GDPR, 0);
                break;
            case CAMPAIGN_TYPE.CCPA:
                type = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.CCPA, 0);
                break;
            default:
                Util.LogError("CampaignType is NULL. How did you get there?");
                break;
        }
        Util.Log($"CampaignType {campaignType} is OK");
        return type;
    }

    private AndroidJavaObject ConstructTargetingParam(string key, string value)
    {
        AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", key, value);
        Util.Log("TargetingParam is OK");
        return targetingParam;
    }

    private AndroidJavaObject ConstructCampaignEnv(CAMPAIGN_ENV environment)
    {
        AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv");
        campaignEnv.Set("value", CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment));
        Util.Log("campaignEnv is OK");
        return campaignEnv;
    }

    private AndroidJavaObject ConstructMessageLanguage(MESSAGE_LANGUAGE lang)
    {
        AndroidJavaObject msgLang = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.MessageLanguage");
        msgLang.Set("value", CSharp2JavaStringEnumMapper.GetMessageLanguageKey(lang));
        Util.Log("MessageLanguage is OK");
        return msgLang;
    }

    private void RunOnUiThread(Action action)
    {
        Util.Log(">>>STARTING RUNNABLE ON UI THREAD!");
        activity.Call("runOnUiThread", new AndroidJavaRunnable(action));
    }

    private void InvokeLoadMessage()
    {
        Util.Log("InvokeLoadMessage() STARTING...");
        try
        {
            consentLib.Call("loadMessage");
            Util.Log($"loadMessage() is OK...");
        }
        catch (Exception ex) { Util.LogError(ex.Message); }
        finally { Util.Log($"InvokeLoadMessage() DONE"); }
    }

    private void InvokeLoadPrivacyManager(string pmId, AndroidJavaObject tab, AndroidJavaObject campaignType, CAMPAIGN_TYPE campaignTypeForLog)
    {
        Util.Log("InvokeLoadPrivacyManager() STARTING...");
        try
        {
            consentLib.Call("loadPrivacyManager", pmId, tab, campaignType);
            Util.Log($"loadPrivacyManager() with {campaignTypeForLog} is OK...");
        }
        catch (Exception ex) { Util.LogError(ex.Message); }
        finally { Util.Log($"InvokeLoadPrivacyManager() with {campaignTypeForLog} DONE"); }
    }

    private void InvokeLoadMessageWithAuthID(string authID)
    {
        Util.Log("loadMessage(authId: String) STARTING...");
        try
        {
            consentLib.Call("loadMessage", authID);
        }
        catch (Exception ex) { Util.LogError(ex.Message); }
        finally { Util.Log("loadMessage(authId: String) DONE"); }
    }

    #region Helper UnityUtils methods usage
    internal static AndroidJavaObject ConvertArrayToList(AndroidJavaObject[] array)
    {
        AndroidJavaClass UnityUtils = new AndroidJavaClass("com.sourcepoint.cmplibrary.util.UnityUtils");
        Util.Log("C# : passing Array to List conversion to Android's UnityUtils...");
        AndroidJavaObject list = UnityUtils.CallStatic<AndroidJavaObject>("targetingParamArrayToList", new AndroidJavaObject[][] { array });
        return list;
    }

    internal static Exception ConvertThrowableToError(AndroidJavaObject rawErr)
    {
        AndroidJavaClass UnityUtils = new AndroidJavaClass("com.sourcepoint.cmplibrary.util.UnityUtils");
        try
        {
            Util.Log("C# : passing Throwable to Exception conversion to Android's UnityUtils...");
            UnityUtils.CallStatic("throwableToException", rawErr);
        }
        catch (AndroidJavaException exception)
        {
            return exception;
        }
        return new NotImplementedException();
    }

    private AndroidJavaObject GetActivity()
    {
        AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        return activity;
    }
    #endregion
}