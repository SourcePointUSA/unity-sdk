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
    Dictionary<PRIVACY_MANAGER_TAB, string> privacyManagerTabToJavaEnumKey;
    Dictionary<CAMPAIGN_ENV, string> campaignEnvToJavaEnumKey;
    Dictionary<MESSAGE_LANGUAGE, string> messageLanguageToJavaKey;
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
        InitializeMapping();
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

    internal static AndroidJavaObject ConvertArrayToList(AndroidJavaObject[] array)
    {
        AndroidJavaClass UnityUtils = new AndroidJavaClass("com.sourcepoint.cmplibrary.util.UnityUtils");
        Util.Log("C# : passing Array to List conversion to Android's UnityUtils...");
        AndroidJavaObject list = UnityUtils.CallStatic<AndroidJavaObject>("targetingParamArrayToList", new AndroidJavaObject[][] { array } );
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

    internal void CallShowView(AndroidJavaObject view)
    {
        consentLib.Call("showView", view);
        Util.Log("C# : View showing passed to Android's ConsentWrapperV6.builder");
    }

    internal void CallRemoveView(AndroidJavaObject view)
    {
        consentLib.Call("removeView", view);
        Util.Log("C# : View removal passed to Android's ConsentWrapperV6.builder");
    }

    private AndroidJavaObject ConsrtuctLib(int accountId, int propertyId, string propertyName, string pmId, PRIVACY_MANAGER_TAB tab/*, MESSAGE_LANGUAGE LANGUAGE*/)
    {
        AndroidJavaObject gdprCampaign = ConstructGDPRCampaign(CAMPAIGN_ENV.PUBLIC);
        AndroidJavaObject ccpaCampaign = ConstructCCPACampaign(CAMPAIGN_ENV.PUBLIC);
        AndroidJavaObject spConfig = ConstructSpConfig_2(accountId, propertyName, new AndroidJavaObject[] { gdprCampaign, ccpaCampaign });
        Util.Log("SpConfig is OK");

        AndroidJavaObject msgLang = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.MessageLanguage");
        msgLang.Set("value", messageLanguageToJavaKey[MESSAGE_LANGUAGE.ENGLISH]);
        Util.Log("MessageLanguage is OK");

        AndroidJavaObject lib = pluginBuilderClass.CallStatic<AndroidJavaObject>("makeConsentLib", spConfig, activity, spClient, msgLang);
        Util.Log("consentLib is OK");
        //lib.Call("setSpClient", spClient);
        //Util.Log("setSpClient is OK");
        return lib;
    }

    private AndroidJavaObject ConstructSpConfig_2(int accountId, string propertyName, AndroidJavaObject[] spCampaigns)
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

    private AndroidJavaObject ConstructSpConfig(int accountId, string propertyName, AndroidJavaObject[] spCampaigns)
    {
        AndroidJavaObject spConfig;
        using (AndroidJavaClass SpConfigDataBuilderClass = new AndroidJavaClass("com.sourcepoint.cmplibrary.creation.SpConfigDataBuilder"))
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

        //AndroidJavaClass SpCampaignClass = new AndroidJavaClass("com.sourcepoint.cmplibrary.model.exposed.SpCampaign");
        //// constructing an array with...            lenght,             contains this class,           initial value for each cell of array
        //IntPtr arrayPtr = AndroidJNI.NewObjectArray(spCampaigns.Length, SpCampaignClass.GetRawClass(), spCampaigns[0].GetRawObject());
        //Util.Log("NewObjectArray is OK");
        //if (spCampaigns.Length > 1)
        //{
        //    //put...                         into    , position, object to put
        //    AndroidJNI.SetObjectArrayElement(arrayPtr, 0, spCampaigns[1].GetRawObject());
        //    Util.Log("SetObjectArrayElement is OK");
        //}

        ////if (arrayPtr.Equals(IntPtr.Zero) || AndroidJNIHelper.ConvertFromJNIArray<Array>(arrayPtr).Length<1)
        ////{
        ////    Debug.LogError("FAIL");
        ////}

        //var arr = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(arrayPtr);

        //AndroidJavaObject spConfig = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpConfig", accountId, propertyName, AndroidJNIHelper.CreateJNIArgArray(new object { spCampaigns[0], spCampaigns[1]} ));
        //AndroidJavaObject spConfig = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpConfig", accountId, propertyName, arr);
        //AndroidJavaObject spConfig = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpConfig", accountId, propertyName, spCampaigns);
    }

    private AndroidJavaObject ConstructGDPRCampaign(/* CAMPAIGN_TYPE campaignType, */ CAMPAIGN_ENV environment)
    {
        //AndroidJavaObject gdprCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.GDPRCampaign", accountId, propertyId, propertyName, pmId);
        AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv");
        campaignEnv.Set("value", campaignEnvToJavaEnumKey[environment]);
        Util.Log("campaignEnv is OK");
        AndroidJavaObject legislation = ConstructCampaignType(CAMPAIGN_TYPE.GDPR);
        //AndroidJavaClass TargetingParam = new AndroidJavaClass("com.sourcepoint.cmplibrary.model.exposed.TargetingParam");
        AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", "location", "EU");
        Util.Log("TargetingParam is OK");
        AndroidJavaObject[] arr = new AndroidJavaObject[] { targetingParam };
        AndroidJavaObject list = ConvertArrayToList(arr);
        //AndroidJavaObject gdprCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", legislation, /*campaignEnv,*/ arr);
        AndroidJavaObject gdprCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", legislation, /*campaignEnv,*/ list);
        Util.Log("GDPRCampaign is OK");
        return gdprCampaign;
    }

    private AndroidJavaObject ConstructGDPRCampaignOld(/* CAMPAIGN_TYPE campaignType, */ CAMPAIGN_ENV environment)
    {
        //AndroidJavaObject gdprCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.GDPRCampaign", accountId, propertyId, propertyName, pmId);
        AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv");
        campaignEnv.Set("value", campaignEnvToJavaEnumKey[environment]);
        Util.Log("campaignEnv is OK");
        AndroidJavaObject legislation = ConstructCampaignType(CAMPAIGN_TYPE.GDPR);
        //AndroidJavaClass TargetingParam = new AndroidJavaClass("com.sourcepoint.cmplibrary.model.exposed.TargetingParam");
        AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", "location", "EU");
        Util.Log("TargetingParam is OK");
        AndroidJavaObject[] arr = new AndroidJavaObject[] { targetingParam };
        AndroidJavaObject gdprCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", legislation, campaignEnv, arr);
        Util.Log("GDPRCampaign is OK");
        return gdprCampaign;
    }

    private AndroidJavaObject ConstructCCPACampaign(/* CAMPAIGN_TYPE campaignType, */ CAMPAIGN_ENV environment)
    {
        //AndroidJavaObject ccpaCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.CCPACampaign", accountId, propertyId, propertyName, pmId);
        AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv");
        campaignEnv.Set("value", campaignEnvToJavaEnumKey[environment]);
        Util.Log("campaignEnv is OK");
        AndroidJavaObject legislation = ConstructCampaignType(CAMPAIGN_TYPE.CCPA);
        //AndroidJavaClass TargetingParam = new AndroidJavaClass("com.sourcepoint.cmplibrary.model.exposed.TargetingParam");
        AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", "location", "EU");
        Util.Log("TargetingParam is OK");
        AndroidJavaObject[] arr = new AndroidJavaObject[] { targetingParam };
        AndroidJavaObject list = ConvertArrayToList(arr);
        //AndroidJavaObject ccpaCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", legislation, /*campaignEnv,*/ arr);
        AndroidJavaObject ccpaCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", legislation, /*campaignEnv,*/ list);
        Util.Log("CCPACampaign is OK");
        return ccpaCampaign;
    }
    
    private AndroidJavaObject ConstructCCPACampaignOld(/* CAMPAIGN_TYPE campaignType, */ CAMPAIGN_ENV environment)
    {
        //AndroidJavaObject ccpaCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.CCPACampaign", accountId, propertyId, propertyName, pmId);
        AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv");
        campaignEnv.Set("value", campaignEnvToJavaEnumKey[environment]);
        Util.Log("campaignEnv is OK");
        AndroidJavaObject legislation = ConstructCampaignType(CAMPAIGN_TYPE.CCPA);
        //AndroidJavaClass TargetingParam = new AndroidJavaClass("com.sourcepoint.cmplibrary.model.exposed.TargetingParam");
        AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", "location", "EU");
        Util.Log("TargetingParam is OK");
        AndroidJavaObject[] arr = new AndroidJavaObject[] { targetingParam };
        AndroidJavaObject ccpaCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", legislation, campaignEnv, arr);
        Util.Log("CCPACampaign is OK");
        return ccpaCampaign;
    }

    private AndroidJavaObject ConstructPrivacyManagerTab(PRIVACY_MANAGER_TAB tab)
    {
        AndroidJavaObject privacyManagerTabK = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.PMTab");
        privacyManagerTabK.Set("key", privacyManagerTabToJavaEnumKey[tab]);
        Util.Log("PMTab is OK");
        return privacyManagerTabK;
    }

    private AndroidJavaObject ConstructCampaignType(CAMPAIGN_TYPE campaignType)
    {
        AndroidJavaObject legit = null;
        switch (campaignType)
        {
            case CAMPAIGN_TYPE.GDPR:
                legit = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.GDPR, 0);
                break;
            case CAMPAIGN_TYPE.CCPA:
                legit = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.CCPA, 0);
                break;
            default:
                Util.LogError("CampaignType is NULL. How did you get there?");
                break;
        }
        Util.Log("CampaignType is OK");
        return legit;
    }

    private AndroidJavaObject ConstructLegislationOld(CAMPAIGN_TYPE campaignType)
    {
        AndroidJavaObject legit = null;
        switch (campaignType)
        {
            case CAMPAIGN_TYPE.GDPR:
                legit = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.Legislation", CAMPAIGN_TYPE_STRING_KEY.GDPR, 0);
                break;
            case CAMPAIGN_TYPE.CCPA:
                legit = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.Legislation", CAMPAIGN_TYPE_STRING_KEY.CCPA, 0);
                break;
            default:
                Util.LogError("Legislation is NULL. How did you get there?");
                break;
        }
        Util.Log("legislation is OK");
        return legit;
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

    private void InitializeMapping()
    {
        InitializePrivacyManagerTabMapping();
        InitializeCampaignEnvMapping();
        InitializeMessageLanguageMapping();
    }

    private void InitializeMessageLanguageMapping()
    {
        messageLanguageToJavaKey = new Dictionary<MESSAGE_LANGUAGE, string>();
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.BULGARIAN, MESSAGE_LANGUAGE_STRING_KEY.BULGARIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CHINESE, MESSAGE_LANGUAGE_STRING_KEY.CHINESE);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CROATIAN, MESSAGE_LANGUAGE_STRING_KEY.CROATIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CZECH, MESSAGE_LANGUAGE_STRING_KEY.CZECH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.DANISH, MESSAGE_LANGUAGE_STRING_KEY.DANISH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.DUTCH, MESSAGE_LANGUAGE_STRING_KEY.DUTCH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ENGLISH, MESSAGE_LANGUAGE_STRING_KEY.ENGLISH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ESTONIAN, MESSAGE_LANGUAGE_STRING_KEY.ESTONIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.FINNISH, MESSAGE_LANGUAGE_STRING_KEY.FINNISH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.FRENCH, MESSAGE_LANGUAGE_STRING_KEY.FRENCH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GAELIC, MESSAGE_LANGUAGE_STRING_KEY.GAELIC);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GERMAN, MESSAGE_LANGUAGE_STRING_KEY.GERMAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GREEK, MESSAGE_LANGUAGE_STRING_KEY.GREEK);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.HUNGARIAN, MESSAGE_LANGUAGE_STRING_KEY.HUNGARIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ICELANDIC, MESSAGE_LANGUAGE_STRING_KEY.ICELANDIC);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ITALIAN, MESSAGE_LANGUAGE_STRING_KEY.ITALIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.JAPANESE, MESSAGE_LANGUAGE_STRING_KEY.JAPANESE);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.LATVIAN, MESSAGE_LANGUAGE_STRING_KEY.LATVIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.LITHUANIAN, MESSAGE_LANGUAGE_STRING_KEY.LITHUANIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.NORWEGIAN, MESSAGE_LANGUAGE_STRING_KEY.NORWEGIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.POLISH, MESSAGE_LANGUAGE_STRING_KEY.POLISH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.PORTUGEESE, MESSAGE_LANGUAGE_STRING_KEY.PORTUGEESE);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ROMANIAN, MESSAGE_LANGUAGE_STRING_KEY.ROMANIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.RUSSIAN, MESSAGE_LANGUAGE_STRING_KEY.RUSSIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SERBIAN_CYRILLIC, MESSAGE_LANGUAGE_STRING_KEY.SERBIAN_CYRILLIC);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SERBIAN_LATIN, MESSAGE_LANGUAGE_STRING_KEY.SERBIAN_LATIN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SLOVAKIAN, MESSAGE_LANGUAGE_STRING_KEY.SLOVAKIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SLOVENIAN, MESSAGE_LANGUAGE_STRING_KEY.SLOVENIAN);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SPANISH, MESSAGE_LANGUAGE_STRING_KEY.SPANISH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SWEDISH, MESSAGE_LANGUAGE_STRING_KEY.SWEDISH);
        messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.TURKISH, MESSAGE_LANGUAGE_STRING_KEY.TURKISH);
    }

    private void InitializeCampaignEnvMapping()
    {
        campaignEnvToJavaEnumKey = new Dictionary<CAMPAIGN_ENV, string>();
        campaignEnvToJavaEnumKey.Add(CAMPAIGN_ENV.PUBLIC, CAMPAIGN_ENV_STRING_KEY.PUBLIC);
        campaignEnvToJavaEnumKey.Add(CAMPAIGN_ENV.STAGE, CAMPAIGN_ENV_STRING_KEY.STAGE);
    }

    private void InitializePrivacyManagerTabMapping()
    {
        privacyManagerTabToJavaEnumKey = new Dictionary<PRIVACY_MANAGER_TAB, string>();
        privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.DEFAULT, PRIVACY_MANAGER_TAB_STRING_KEY.DEFAULT);
        privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.PURPOSES, PRIVACY_MANAGER_TAB_STRING_KEY.PURPOSES);
        privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.VENDORS, PRIVACY_MANAGER_TAB_STRING_KEY.VENDORS);
        privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.FEATURES, PRIVACY_MANAGER_TAB_STRING_KEY.FEATURES);
    }

    private AndroidJavaObject GetActivity()
    {
        AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        return activity;
    }
}