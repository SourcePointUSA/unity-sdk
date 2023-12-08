using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using System.Text.Json;
using ConsentManagementProviderLib.Json;

namespace ConsentManagementProviderLib.iOS
{
    internal class CMPiOSListenerHelper : MonoBehaviour
    {

#if UNITY_IOS && !UNITY_EDITOR_OSX
    [DllImport("__Internal")]
    private static extern void _setCallbackDefault(Action<string> callback);
    [DllImport("__Internal")]
    private static extern void _setCallbackOnConsentReady(Action<string> callback);
    [DllImport("__Internal")]
    private static extern void _setCallbackOnConsentUIReady(Action<string> callback);
    [DllImport("__Internal")]
    private static extern void _setCallbackOnConsentAction(Action<string> callback);
    [DllImport("__Internal")]
    private static extern void _setCallbackOnConsentUIFinished(Action<string> callback);
    [DllImport("__Internal")]
    private static extern void _setCallbackOnErrorCallback(Action<string> callback);
    [DllImport("__Internal")]
    private static extern void _setCallbackOnCustomConsent(Action<string> callback);
#endif

        public static CMPiOSListenerHelper self;
        private Action<GdprConsent> onCustomConsentsGDPRSuccessAction;
        internal GdprConsent customGdprConsent = null;
        internal SpConsents _spConsents = null;
        
        private void Awake()
        {
            self=this;
            gameObject.name = "CMPiOSListenerHelper";
#if UNITY_IOS && !UNITY_EDITOR_OSX
        CmpDebugUtil.Log("Constructing CMPiOSListenerHelper game object...");
        DontDestroyOnLoad(this.gameObject);
        _setCallbackDefault(Callback);
        _setCallbackOnConsentReady(OnConsentReady);
        _setCallbackOnConsentUIReady(OnConsentUIReady);
        _setCallbackOnConsentAction(OnConsentAction);
        _setCallbackOnConsentUIFinished(OnConsentUIFinished);
        _setCallbackOnErrorCallback(OnErrorCallback);
        _setCallbackOnCustomConsent(OnCustomConsentGDPRCallback);
#endif
        }

        internal void SetCustomConsentsGDPRSuccessAction(Action<GdprConsent> action)
        {
            this.onCustomConsentsGDPRSuccessAction = action;
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        static void Callback(string message)
        {
            CmpDebugUtil.Log("IOS_CALLBACK_RECEIVED: " + message);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        static void OnConsentReady(string message)
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
                self.SaveConsent(spConsents);
                ConsentMessenger.Broadcast<IOnConsentReady>(spConsents);
            }
        }

        void SaveConsent(SpConsents consent)
        {
            _spConsents = consent;
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        static void OnConsentUIReady(string message)
        {
            CmpDebugUtil.Log("OnConsentUIReady IOS_CALLBACK_RECEIVED: " + message);
            ConsentMessenger.Broadcast<IOnConsentUIReady>(message);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        static void OnConsentAction(string message)
        {
            CmpDebugUtil.Log("OnConsentAction IOS_CALLBACK_RECEIVED: " + message);
            SpActionWrapper wrapped = JsonSerializer.Deserialize<SpActionWrapper>(message);
            CONSENT_ACTION_TYPE unwrappedType = (CONSENT_ACTION_TYPE) Convert.ToInt32(wrapped.type);
            string customActionId = wrapped.customActionId.ToString();
            SpAction spAction = new SpAction(unwrappedType, customActionId);
            ConsentMessenger.Broadcast<IOnConsentAction>(spAction);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        static void OnConsentUIFinished(string message)
        {
            CmpDebugUtil.Log("OnConsentUIFinished IOS_CALLBACK_RECEIVED: " + message);
            ConsentMessenger.Broadcast<IOnConsentUIFinished>();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        static void OnErrorCallback(string jsonError)
        {
            CmpDebugUtil.LogError("OnErrorCallback IOS_CALLBACK_RECEIVED: " + jsonError);
            Exception ex = new Exception(jsonError);
            ConsentMessenger.Broadcast<IOnConsentError>(ex);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        static void OnCustomConsentGDPRCallback(string jsonSPGDPRConsent)
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
                    self.onCustomConsentsGDPRSuccessAction?.Invoke(null);
                }
                else
                {
                    self.customGdprConsent = unwrapped;
                    self.onCustomConsentsGDPRSuccessAction?.Invoke(unwrapped);
                }
            }
        }
    }
}