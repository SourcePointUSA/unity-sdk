using System;
using UnityEngine;

namespace GdprConsentLib
{
    public class SpClientProxy : AndroidJavaProxy
    {
        static readonly string NativeJavaInterfaceName = "com.sourcepoint.cmplibrary.UnitySpClient";

        public SpClientProxy() : base(new AndroidJavaClass(NativeJavaInterfaceName)) { }

        /**
         * It is invoked when the WebView has been already loaded with all the consent Info
         */
        void onUIReady(AndroidJavaObject view)
        {
            DebugUtil.Log("I've reached the C# onUIReady");
            ConsentWrapperV6.Instance.CallShowView(view);
            ConsentMessenger.Broadcast<IOnConsentUIReadyEventHandler>();
        }

        /**
         * It is invoked when the interaction with WebView is done, consent sent and we are ready to close the WebView
         */
        void onUIFinished(AndroidJavaObject view)
        {
            DebugUtil.Log("I've reached the C# onUIFinished");
            ConsentWrapperV6.Instance.CallRemoveView(view);
            ConsentMessenger.Broadcast<IOnConsentUIFinishedEventHandler>();
        }

        void onAction(AndroidJavaObject view, AndroidJavaObject actionType)
        {
            CONSENT_ACTION_TYPE unwrappedType = (CONSENT_ACTION_TYPE)actionType.Call<int>("getCode");
            DebugUtil.Log("I've reached the C# onAction: " + unwrappedType);
            ConsentMessenger.Broadcast<IOnConsentActionEventHandler>(unwrappedType);
        }

        void onConsentReady(string spConsents) 
        {
            DebugUtil.Log("I've reached the C# onConsentReady with json string: " + spConsents);
            ConsentMessenger.Broadcast<IOnConsentReadyEventHandler>(spConsents);
        }

        void onError(AndroidJavaObject rawThrowableObject)
        {
            DebugUtil.Log("I've reached the C# onError : " + rawThrowableObject.ToString());
            Exception exception = UnityUtils.ConvertThrowableToError(rawThrowableObject);
            DebugUtil.Log("Exception converted successfully : " + exception.ToString());
            ConsentMessenger.Broadcast<IOnConsentErrorEventHandler>(exception);
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
            DebugUtil.Log("I've reached the C# onMessageReady");
            //TODO
            //ConsentMessenger.Broadcast<IOnConsentMessageReady>(unwrappedspMessage);
        }
        #endregion
    }
}