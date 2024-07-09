using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using ConsentManagementProviderLib.Json;
using System.Collections;
using System.Collections.Generic;

namespace ConsentManagementProviderLib.iOS
{
    internal class CMPiOSListenerHelper : MonoBehaviour
    {

#if UNITY_IOS && !UNITY_EDITOR_OSX
    [DllImport("__Internal")]
    private static extern void _setCallback(Action<string> callback, string callbackType);
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
            SetBridgeCallbacks();
#endif
        }

        internal void SetBridgeCallbacks()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            _setCallback(Callback, CALLBACK_TYPE.Default);
            _setCallback(OnConsentReady, CALLBACK_TYPE.OnConsentReady);
            _setCallback(OnConsentUIReady, CALLBACK_TYPE.OnConsentUIReady);
            _setCallback(OnConsentAction, CALLBACK_TYPE.OnConsentAction);
            _setCallback(OnConsentUIFinished, CALLBACK_TYPE.OnConsentUIFinished);
            _setCallback(OnConsentSPFinished, CALLBACK_TYPE.OnSPFinished);
            _setCallback(OnErrorCallback, CALLBACK_TYPE.OnErrorCallback);
            _setCallback(OnCustomConsentGDPRCallback, CALLBACK_TYPE.OnCustomConsent);
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
                spConsents = JsonUnwrapperIOS.UnwrapSpConsents(message);
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
            using StringReader stringReader = new StringReader(message);
            using Newtonsoft.Json.JsonTextReader reader = new Newtonsoft.Json.JsonTextReader(stringReader);

            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            SpActionWrapper wrapped = serializer.Deserialize<SpActionWrapper>(reader);

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
        static void OnConsentSPFinished(string message)
        {
            CmpDebugUtil.Log("OnConsentSpFinished IOS_CALLBACK_RECEIVED: " + message);
            ConsentMessenger.Broadcast<IOnConsentSpFinished>();
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
                unwrapped = JsonUnwrapperIOS.UnwrapGdprConsent(jsonSPGDPRConsent);
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

        public void Dispose()
        {
            if (self != null)
            {
                Destroy(self.gameObject);
                self = null;
            }
        }
    }
}