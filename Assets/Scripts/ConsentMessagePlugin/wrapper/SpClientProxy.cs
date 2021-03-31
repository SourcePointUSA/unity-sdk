using GdprConsentLib;
using System;
using UnityEngine;

namespace GdprConsentLib
{
    public class SpClientProxy : AndroidJavaProxy
    {
        static readonly string NativeJavaInterfaceName = "com.sourcepoint.cmplibrary.SpClient";

        public SpClientProxy() : base(new AndroidJavaClass(NativeJavaInterfaceName)) { }

        /**
         * It is invoked when the WebView has been already loaded with all the consent Info
         */
        void onUIReady(AndroidJavaObject view)
        {
            Util.Log("I've reached the C# onUIReady");
            ConsentWrapperV6.Instance.CallShowView(view);
            ConsentMessenger.Broadcast<IOnConsentUIReadyEventHandler>();
        }

        /**
         * It is invoked when the interaction with WebView is done, consent sent and we are ready to close the WebView
         */
        void onUIFinished(AndroidJavaObject view)
        {
            Util.Log("I've reached the C# onUIFinished");
            ConsentWrapperV6.Instance.CallRemoveView(view);
            ConsentMessenger.Broadcast<IOnConsentUIFinishedEventHandler>();
        }

        void onAction(AndroidJavaObject view, AndroidJavaObject actionType)
        {
            CONSENT_ACTION_TYPE unwrappedType = (CONSENT_ACTION_TYPE)actionType.Call<int>("getCode");
            Util.Log("I've reached the C# onAction: " + unwrappedType);
            ConsentMessenger.Broadcast<IOnConsentActionEventHandler>(unwrappedType);
        }

        void onConsentReady(AndroidJavaObject spConsents)
        {
            Util.Log("I've reached the C# onConsentReady");
            try
            {
                //In progress...
                
                //unwrappedSpConsent is c# object which takes java.lang.Object in constructor; constructor accesses java.lang.Object fields and set result to C# object 
                //SpConsents unwrappedSpConsent = new SpConsents(spConsents);
                //Util.LogError("Works!");
                //ConsentMessenger.Broadcast<IOnConsentReadyEventHandler>(unwrappedSpConsent);
            }
            catch (Exception ex) { Util.LogError(ex.Message); }
        }

        void onConsentReady(string spConsents) 
        {
            Util.LogError("I've reached the C# onConsentReady with string: " + spConsents);
            //In progress...
        }

        void onError(AndroidJavaObject rawThrowableObject)
        {
            Util.Log("I've reached the C# onError : " + rawThrowableObject.ToString());
            Exception exception = ConsentWrapperV6.ConvertThrowableToError(rawThrowableObject);
            Util.Log("Exception converted successfully : " + exception.ToString());
            ConsentMessenger.Broadcast<IOnConsentErrorEventHandler>(exception);
        }
        
        #region Not Implemented
        /**
         * It is invoked when the message is available to the client App
         */
        void onMessageReady(AndroidJavaObject spMessage)//(message: SPMessage)
        {
            Util.Log("I've reached the C# onMessageReady");
            //TODO
            //ConsentMessenger.Broadcast<IOnConsentMessageReady>(unwrappedspMessage);
        }
        #endregion
    }
}