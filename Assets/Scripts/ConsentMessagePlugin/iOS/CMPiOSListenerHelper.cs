using System;
using System.Runtime.InteropServices;
using GdprConsentLib;
using UnityEngine;

public class CMPiOSListenerHelper : MonoBehaviour
{
    
#if UNITY_IOS && !UNITY_EDITOR_OSX
    [DllImport("__Internal")]
    private static extern void _setUnityCallback(string gameObjectName);//, string methodName);
#endif
    
    private void Awake()
    {
        gameObject.name = "CMPiOSListenerHelper";
#if UNITY_IOS && !UNITY_EDITOR_OSX
        DebugUtil.Log("Constructing CMPiOSListenerHelper game object...");
        _setUnityCallback(gameObject.name);
#endif
    }

    void Callback(string message)
    {
        DebugUtil.Log("IOS_CALLBACK_RECEIVED: " + message);
    }  
    
    void OnConsentReady(string message)
    {
        DebugUtil.Log("OnConsentReady IOS_CALLBACK_RECEIVED: " + message);
    }

    void OnConsentUIReady(string message)
    {
        DebugUtil.Log("OnConsentUIReady IOS_CALLBACK_RECEIVED: " + message);
        ConsentMessenger.Broadcast<IOnConsentUIReadyEventHandler>(message);
    }

    void OnConsentAction(string message)
    {
        DebugUtil.Log("OnConsentAction IOS_CALLBACK_RECEIVED: " + message);
        CONSENT_ACTION_TYPE actionType = (CONSENT_ACTION_TYPE) Convert.ToInt32(message);
        ConsentMessenger.Broadcast<IOnConsentActionEventHandler>(actionType);
    }
    
    void OnConsentUIFinished(string message)
    {
        DebugUtil.Log("OnConsentUIFinished IOS_CALLBACK_RECEIVED: " + message);
        ConsentMessenger.Broadcast<IOnConsentUIFinishedEventHandler>();
    }
    
    void OnErrorCallback(string jsonError)
    {
        DebugUtil.LogError("OnErrorCallback IOS_CALLBACK_RECEIVED: " + jsonError);
        Exception ex = new Exception(jsonError);
        ConsentMessenger.Broadcast<IOnConsentErrorEventHandler>(ex);
    }
}
