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
        pluginBuilderClass = new AndroidJavaClass(androidPluginName);
        Util.Log("plugin class is OK");
        activity = GetActivity();
        Util.Log("Activity is OK");
        spClient = new SpClientProxy();
        Util.Log("spClient is OK");
    }

    public void CallConsentAAR(int accountId, int propertyId, string propertyName, string pmId, PRIVACY_MANAGER_TAB tab, string authID = null)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                consentLib = ConsrtuctLib(accountId, propertyId, propertyName, pmId, tab);
                if (string.IsNullOrEmpty(authID))
                {
                    RunOnUiThread(delegate { InvokeLoadMessage(pmId, tab); });
                }
                else
                {
                    RunOnUiThread( delegate { InvokeLoadMessageWithAuthID(authID); });
                }
            }
            catch (Exception e)
            {
                Util.LogError(e.Message);
            }
        }
#endif
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

    private AndroidJavaObject ConsrtuctLib(int accountId, int propertyId, string propertyName, string pmId, PRIVACY_MANAGER_TAB tab/*, MESSAGE_LANGUAGE LANGUAGE*/)
    {
        AndroidJavaObject gdprCampaign = ConstructGDPRCampaign(CAMPAIGN_ENV.PUBLIC);
        AndroidJavaObject ccpaCampaign = ConstructCCPACampaign(CAMPAIGN_ENV.PUBLIC);
        AndroidJavaObject spConfig = ConstructSpConfig(accountId, propertyName, new AndroidJavaObject[] { gdprCampaign, ccpaCampaign });
        Util.Log("SpConfig is OK");
       
        AndroidJavaObject lib = pluginBuilderClass.CallStatic<AndroidJavaObject>("makeConsentLib", spConfig, activity, spClient/*, msgLang*/);
        Util.Log("consentLib is OK");
        return lib;
    }

    private AndroidJavaObject ConstructSpConfig(int accountId, string propertyName, AndroidJavaObject[] spCampaigns)
    {
        AndroidJavaObject spConfig;
        using (AndroidJavaObject SpConfigDataBuilderClass = new AndroidJavaObject("com.sourcepoint.cmplibrary.creation.SpConfigDataBuilder"))
        {
            SpConfigDataBuilderClass.Call<AndroidJavaObject>("addAccountId", accountId);
            Util.Log("addAccountId is OK");
            SpConfigDataBuilderClass.Call<AndroidJavaObject>("addPropertyName", propertyName);
            Util.Log("addPropertyName is OK");

            foreach (AndroidJavaObject camp in spCampaigns)
            {
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addCampaign", camp);
                Util.Log("addCampaign is OK");
            }

            spConfig = SpConfigDataBuilderClass.Call<AndroidJavaObject>("build");
            Util.Log("build() is OK");
        }
        return spConfig;
    }

    private AndroidJavaObject ConstructGDPRCampaign(/* CAMPAIGN_TYPE campaignType, */ CAMPAIGN_ENV environment)
    {
        AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv");
        campaignEnv.Set("value", CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment));
        Util.Log("campaignEnv is OK");
        AndroidJavaObject legislation = ConstructCampaignType(CAMPAIGN_TYPE.GDPR);
        AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", "location", "EU");
        Util.Log("TargetingParam is OK");
        AndroidJavaObject[] arr = new AndroidJavaObject[] { targetingParam };
        AndroidJavaObject list = ConvertArrayToList(arr);
        AndroidJavaObject gdprCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", legislation, /*campaignEnv,*/ list);
        Util.Log("GDPRCampaign is OK");
        return gdprCampaign;
    }

    private AndroidJavaObject ConstructCCPACampaign(/* CAMPAIGN_TYPE campaignType, */ CAMPAIGN_ENV environment)
    {
        AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv");
        campaignEnv.Set("value", CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment));
        Util.Log("campaignEnv is OK");
        AndroidJavaObject legislation = ConstructCampaignType(CAMPAIGN_TYPE.CCPA);
        AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", "location", "EU");
        Util.Log("TargetingParam is OK");
        AndroidJavaObject[] arr = new AndroidJavaObject[] { targetingParam };
        AndroidJavaObject list = ConvertArrayToList(arr);
        AndroidJavaObject ccpaCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", legislation, /*campaignEnv,*/ list);
        Util.Log("CCPACampaign is OK");
        return ccpaCampaign;
    }
    
    private AndroidJavaObject ConstructPrivacyManagerTab(PRIVACY_MANAGER_TAB tab)
    {
        AndroidJavaObject privacyManagerTabK = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.PMTab");
        privacyManagerTabK.Set("key", CSharp2JavaStringEnumMapper.GetPrivacyManagerTabKey(tab));
        Util.Log("PMTab is OK");
        return privacyManagerTabK;
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
        Util.Log("CampaignType is OK");
        return type;
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

    private void InvokeLoadMessage(string pmId, PRIVACY_MANAGER_TAB tab)
    {
        Util.Log("InvokeLoadMessage() STARTING...");
        try
        {
            var pmTab = ConstructPrivacyManagerTab(tab);
            var legitGDPR = ConstructCampaignType(CAMPAIGN_TYPE.GDPR);
            var legitCCPA = ConstructCampaignType(CAMPAIGN_TYPE.CCPA);
            //consentLib.Call("loadPrivacyManager", pmId, pmTab, legitGDPR);

            consentLib.Call("loadPrivacyManager", "13111", pmTab, legitGDPR);
            Util.Log("loadPrivacyManager() with GDPR is OK...");

            //if one by one >>> Oh no, an error! java.lang.Exception: com.sourcepoint.cmplibrary.exception.InvalidResponseWebMessageException: Error trying to build the gdpr body to send consents.
            //java.lang.Exception: com.sourcepoint.cmplibrary.exception.InvalidRequestException: {"err":"Localstate does not have CCPA attribute."}

            consentLib.Call("loadPrivacyManager", "14967", pmTab, legitCCPA);
            Util.Log("loadPrivacyManager() with CCPA is OK...");

            consentLib.Call("loadMessage");
            Util.Log("loadMessage() is OK...");
        }
        catch (Exception ex) { Util.LogError(ex.Message); }
        finally { Util.Log("InvokeLoadMessage() DONE"); }
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