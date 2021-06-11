using System;
using System.Runtime.InteropServices;
using ConsentManagementProviderLib;
using UnityEngine;

public class CMPiOSListenerHelper : MonoBehaviour
{

#if UNITY_IOS && !UNITY_EDITOR_OSX
    [DllImport("__Internal")]
    private static extern void _setUnityCallback(string gameObjectName);
#endif

    private Action<String> onCustomConsentsGDPRSuccessAction;

    private void Awake()
    {
        gameObject.name = "CMPiOSListenerHelper";
#if UNITY_IOS && !UNITY_EDITOR_OSX
        CmpDebugUtil.Log("Constructing CMPiOSListenerHelper game object...");
        DontDestroyOnLoad(this.gameObject);
        _setUnityCallback(gameObject.name);
#endif
    }

    internal void SetCustomConsentsGDPRSuccessAction(Action<string> action)
    {
        this.onCustomConsentsGDPRSuccessAction = action;
    }

    void Callback(string message)
    {
        CmpDebugUtil.Log("IOS_CALLBACK_RECEIVED: " + message);
    }  
    
    void OnConsentReady(string message)
    {
        CmpDebugUtil.Log("OnConsentReady IOS_CALLBACK_RECEIVED: " + message);
        ConsentMessenger.Broadcast<IOnConsentMessageReady>();
    }

    void OnConsentUIReady(string message)
    {
        CmpDebugUtil.Log("OnConsentUIReady IOS_CALLBACK_RECEIVED: " + message);
        ConsentMessenger.Broadcast<IOnConsentUIReadyEventHandler>(message);
    }

    void OnConsentAction(string message)
    {
        CmpDebugUtil.Log("OnConsentAction IOS_CALLBACK_RECEIVED: " + message);
        CONSENT_ACTION_TYPE actionType = (CONSENT_ACTION_TYPE) Convert.ToInt32(message);
        ConsentMessenger.Broadcast<IOnConsentActionEventHandler>(actionType);
    }
    
    void OnConsentUIFinished(string message)
    {
        CmpDebugUtil.Log("OnConsentUIFinished IOS_CALLBACK_RECEIVED: " + message);
        ConsentMessenger.Broadcast<IOnConsentUIFinishedEventHandler>();
    }
    
    void OnErrorCallback(string jsonError)
    {
        CmpDebugUtil.LogError("OnErrorCallback IOS_CALLBACK_RECEIVED: " + jsonError);
        Exception ex = new Exception(jsonError);
        ConsentMessenger.Broadcast<IOnConsentErrorEventHandler>(ex);
    }

    void OnCustomConsentGDPRCallback(string jsonSPGDPRConsent)
    {
        CmpDebugUtil.Log("OnCustomConsentGDPRCallback IOS_CALLBACK_RECEIVED: " + jsonSPGDPRConsent);
        //TODO: unwrapping
        onCustomConsentsGDPRSuccessAction?.Invoke(jsonSPGDPRConsent);
    }
}
