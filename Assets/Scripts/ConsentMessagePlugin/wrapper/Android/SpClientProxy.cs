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

        void onAction(AndroidJavaObject view, AndroidJavaObject actionType)
        {
            CONSENT_ACTION_TYPE unwrappedType = (CONSENT_ACTION_TYPE)actionType.Call<int>("getCode");
            CmpDebugUtil.Log("I've reached the C# onAction: " + unwrappedType);
            ConsentMessenger.Broadcast<IOnConsentAction>(unwrappedType);
        }

        void onConsentReady(string spConsents) 
        {
            CmpDebugUtil.Log("I've reached the C# onConsentReady with json string: " + spConsents);
            SpConsents consents = JsonUnwrapper.UnwrapSpConsentsAndroid(spConsents);
            _spConsents = consents;
            ConsentMessenger.Broadcast<IOnConsentReady>(consents);
        }

        void onError(AndroidJavaObject rawThrowableObject)
        {
            CmpDebugUtil.Log("I've reached the C# onError : " + rawThrowableObject.ToString());
            Exception exception = CmpJavaToUnityUtils.ConvertThrowableToError(rawThrowableObject);
            CmpDebugUtil.Log("Exception converted successfully : " + exception.ToString());
            ConsentMessenger.Broadcast<IOnConsentError>(exception);
        }

        #region Not implemented or implemented partially
        void onConsentReady(AndroidJavaObject spConsents)
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