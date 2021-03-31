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
        InitializePrivacyManagerTabMapping();
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
                    RunOnUiThread(InvokeLoadMessage);
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

    internal static Exception ConvertThrowableToError(AndroidJavaObject rawErr)
    {
        AndroidJavaClass SpUtils = new AndroidJavaClass("com.sourcepoint.cmplibrary.util.SpUtils");
        try
        {
            Util.Log("C# : passing Throwable to Exception conversion to Android's SpUtils...");
            SpUtils.CallStatic("throwableToException", rawErr);
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

    private AndroidJavaObject ConsrtuctLib(int accountId, int propertyId, string propertyName, string pmId, PRIVACY_MANAGER_TAB tab)
    {
        AndroidJavaObject privacyManagerTabK = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.PrivacyManagerTabK");
        privacyManagerTabK.Set("key", privacyManagerTabToJavaEnumKey[tab]);
        Util.Log("privacyManagerTabK is OK");
        AndroidJavaObject gdprCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.GDPRCampaign", accountId, propertyId, propertyName, pmId);
        AndroidJavaObject ccpaCampaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.CCPACampaign", accountId, propertyId, propertyName, pmId);
        AndroidJavaObject lib = pluginBuilderClass.CallStatic<AndroidJavaObject>("makeConsentLib", gdprCampaign, ccpaCampaign, activity, privacyManagerTabK);
        Util.Log("consentLib is OK");
        lib.Call("setSpClient", spClient);
        Util.Log("setSpClient is OK");
        return lib;
    }

    private void RunOnUiThread(Action action)
    {
        Util.Log(">>>STARTING RUNNABLE ON UI THREAD!");
        activity.Call("runOnUiThread", new AndroidJavaRunnable(action));
    }

    private void InvokeLoadMessage()
    {
        Util.Log("loadMessage() STARTING...");
        try
        {
            consentLib.Call("loadMessage");
        }
        catch (Exception ex) { Util.LogError(ex.Message); }
        finally { Util.Log("loadMessage() DONE"); }
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