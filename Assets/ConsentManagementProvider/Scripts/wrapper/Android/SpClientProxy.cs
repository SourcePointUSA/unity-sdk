using System;
using ConsentManagementProviderLib.Json;
using UnityEngine;

namespace ConsentManagementProviderLib.Android
{
    internal class SpClientProxy : AndroidJavaProxy
    {
        static readonly string NativeJavaInterfaceName = "com.sourcepoint.cmplibrary.UnitySpClient";
        internal SpConsents _spConsents = null;
        
        public SpClientProxy() : base(new AndroidJavaClass(NativeJavaInterfaceName)) { }

        /**
         * It is invoked when the WebView has been already loaded with all the consent Info
         */
        void onUIReady(AndroidJavaObject view)
        {
            CmpDebugUtil.Log("I've reached the C# onUIReady");
            ConsentWrapperAndroid.Instance.CallShowView(view);
            ConsentMessenger.Broadcast<IOnConsentUIReady>();
        }

        /**
         * It is invoked when the interaction with WebView is done, consent sent and we are ready to close the WebView
         */
        void onUIFinished(AndroidJavaObject view)
        {
            CmpDebugUtil.Log("I've reached the C# onUIFinished");
            ConsentWrapperAndroid.Instance.CallRemoveView(view);
            ConsentMessenger.Broadcast<IOnConsentUIFinished>();
        }

        AndroidJavaObject onAction(AndroidJavaObject view, AndroidJavaObject actionType)
        {
            CmpDebugUtil.Log("I've reached the C# onAction!");
            
            CmpDebugUtil.Log("Trying to unwrap ActionType... ");
            AndroidJavaObject wrapper = actionType.Call<AndroidJavaObject>("getActionType");
            CONSENT_ACTION_TYPE unwrappedType = (CONSENT_ACTION_TYPE)wrapper.Call<int>("getCode");
            CmpDebugUtil.Log("Unwrapped ActionType is: " + unwrappedType);

            string customActionId = actionType.Call<string>("getCustomActionId");
            CmpDebugUtil.Log("Unwrapped CustomActionId is: " + customActionId);

            SpAction spAction = new SpAction(unwrappedType, customActionId);

            CmpDebugUtil.Log("Trying to put \"pb_key\", \"pb_value\" in pubData");
            AndroidJavaObject pubData = actionType.Call<AndroidJavaObject>("getPubData");
            pubData.Call<AndroidJavaObject>("put", "pb_key", "pb_value");
            CmpDebugUtil.Log("PUT IS SUCCESSFUL");

            ConsentMessenger.Broadcast<IOnConsentAction>(spAction);
            CmpDebugUtil.Log("Now I'll return actionType back to Java...");
            return actionType;
        }

        void onConsentReady(string spConsents) 
        {
            CmpDebugUtil.Log("I've reached the C# onConsentReady with json string: " + spConsents);
            SpConsents consents = JsonUnwrapper.UnwrapSpConsentsAndroid(spConsents);
            _spConsents = consents;
            ConsentMessenger.Broadcast<IOnConsentReady>(consents);
        }
        
        /**
         * It is invoked when the interaction with native WebView is done, consent sent, JSON received and CMP is ready to close the WebView
         */
        void onSpFinished(string spConsents)
        {
            CmpDebugUtil.ForceEnableNextCmpLog();
            CmpDebugUtil.LogWarning($"I've reached the C# onSpFinished with JSON spConsents={spConsents}");
            Console.WriteLine($"spConsents= `{spConsents}");

            try
            {
                SpConsents consents = JsonUnwrapper.UnwrapSpConsentsAndroid(spConsents);
                _spConsents = consents;
                ConsentMessenger.Broadcast<IOnConsentSpFinished>(consents);
            }
            catch (Exception e)
            {
                ConsentMessenger.Broadcast<IOnConsentError>(e);
            }
        }

        void onError(AndroidJavaObject rawThrowableObject)
        {
            CmpDebugUtil.ForceEnableNextCmpLog();
            CmpDebugUtil.LogError("I've reached the C# onError : " + rawThrowableObject.ToString());
            Exception exception = CmpJavaToUnityUtils.ConvertThrowableToError(rawThrowableObject);
            CmpDebugUtil.ForceEnableNextCmpLog();
            CmpDebugUtil.LogError("Exception converted successfully : " + exception.ToString());
            ConsentMessenger.Broadcast<IOnConsentError>(exception);
        }

        #region Not implemented or implemented partially
        void onConsentReady(AndroidJavaObject spConsents)
        {
        }

        void onSpFinished(AndroidJavaObject spConsents)
        {
        }

        /**
         * It is invoked when the message is available to the client App
         */
        void onMessageReady(AndroidJavaObject spMessage)//(message: SPMessage)
        {
            //TODO
            CmpDebugUtil.Log("I've reached the C# onMessageReady");
            //ConsentMessenger.Broadcast<IOnConsentMessageReady>(unwrappedspMessage);
        }
        #endregion
    }
}
