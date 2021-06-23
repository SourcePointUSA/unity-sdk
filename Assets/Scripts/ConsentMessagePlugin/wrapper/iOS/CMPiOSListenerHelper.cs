using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Text.Json;
using ConsentManagementProviderLib.Json;

namespace ConsentManagementProviderLib.iOS
{
    internal class CMPiOSListenerHelper : MonoBehaviour
    {

#if UNITY_IOS && !UNITY_EDITOR_OSX
    [DllImport("__Internal")]
    private static extern void _setUnityCallback(string gameObjectName);
#endif

        private Action<GdprConsent> onCustomConsentsGDPRSuccessAction;
        internal GdprConsent customGdprConsent = null;
        internal SpConsents _spConsents = null;
        
        private void Awake()
        {
            gameObject.name = "CMPiOSListenerHelper";
#if UNITY_IOS && !UNITY_EDITOR_OSX
        CmpDebugUtil.Log("Constructing CMPiOSListenerHelper game object...");
        DontDestroyOnLoad(this.gameObject);
        _setUnityCallback(gameObject.name);
#endif
        }

        internal void SetCustomConsentsGDPRSuccessAction(Action<GdprConsent> action)
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
            SpConsents spConsents = null;
            try
            { 
                spConsents = JsonUnwrapper.UnwrapSpConsents(message);
            }
            catch (Exception ex)
            {
                 Debug.LogError(
                    "Something went wrong while parsing the json data; null will be returned. \n Exception message: " +
                    ex.Message);
            }
            finally
            {
                _spConsents = spConsents;
                ConsentMessenger.Broadcast<IOnConsentReady>(spConsents);
            }
        }

        void OnConsentUIReady(string message)
        {
            CmpDebugUtil.Log("OnConsentUIReady IOS_CALLBACK_RECEIVED: " + message);
            ConsentMessenger.Broadcast<IOnConsentUIReady>(message);
        }

        void OnConsentAction(string message)
        {
            CmpDebugUtil.Log("OnConsentAction IOS_CALLBACK_RECEIVED: " + message);
            CONSENT_ACTION_TYPE actionType = (CONSENT_ACTION_TYPE) Convert.ToInt32(message);
            ConsentMessenger.Broadcast<IOnConsentAction>(actionType);
        }

        void OnConsentUIFinished(string message)
        {
            CmpDebugUtil.Log("OnConsentUIFinished IOS_CALLBACK_RECEIVED: " + message);
            ConsentMessenger.Broadcast<IOnConsentUIFinished>();
        }

        void OnErrorCallback(string jsonError)
        {
            CmpDebugUtil.LogError("OnErrorCallback IOS_CALLBACK_RECEIVED: " + jsonError);
            Exception ex = new Exception(jsonError);
            ConsentMessenger.Broadcast<IOnConsentError>(ex);
        }

        void OnCustomConsentGDPRCallback(string jsonSPGDPRConsent)
        {
            CmpDebugUtil.Log("OnCustomConsentGDPRCallback IOS_CALLBACK_RECEIVED: " + jsonSPGDPRConsent);
            GdprConsent unwrapped = null;
            try
            {
                unwrapped = JsonUnwrapper.UnwrapGdprConsent(jsonSPGDPRConsent);
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    "Something went wrong while parsing the json data; null will be returned. \n Exception message: " +
                    ex.Message);
            }
            finally
            {
                if (unwrapped == null)
                {
                    onCustomConsentsGDPRSuccessAction?.Invoke(null);
                }
                else
                {
                    customGdprConsent = unwrapped;
                    onCustomConsentsGDPRSuccessAction?.Invoke(unwrapped);
                }
            }
        }
    }
}