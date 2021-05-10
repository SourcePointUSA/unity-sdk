using GdprConsentLib;
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

        void onConsentReady(string spConsents) 
        {
            Util.Log("I've reached the C# onConsentReady with json string: " + spConsents);
            //SPCCPAConsent spCcpaConsent = JsonUtility.FromJson<SPCCPAConsent>(spConsents);
            //Util.LogError(">>>spCcpaConsent.applies "+spCcpaConsent.applies+ " , consent is... " + spCcpaConsent.consent);
            //if (spCcpaConsent.consent != null)
            //{
            //    Util.LogError(">>>spCcpaConsent.consent.status is... " + spCcpaConsent.consent.status);
            //    Util.LogError(">>>spCcpaConsent.consent.uspstring is... " + spCcpaConsent.consent.uspstring);
            //    Util.LogError(">>>spCcpaConsent.consent.rejectedVendors is... " + (spCcpaConsent.consent.rejectedVendors != null ? spCcpaConsent.consent.rejectedVendors.Count.ToString() : "NULL"));
            //    Util.LogError(">>>spCcpaConsent.consent.rejectedCategories is... " + (spCcpaConsent.consent.rejectedCategories != null ? spCcpaConsent.consent.rejectedCategories.Count.ToString() : "NULL"));
            //}
            ConsentMessenger.Broadcast<IOnConsentReadyEventHandler>(spConsents);
        }

        void onError(AndroidJavaObject rawThrowableObject)
        {
            Util.Log("I've reached the C# onError : " + rawThrowableObject.ToString());
            Exception exception = UnityUtils.ConvertThrowableToError(rawThrowableObject);
            Util.Log("Exception converted successfully : " + exception.ToString());
            ConsentMessenger.Broadcast<IOnConsentErrorEventHandler>(exception);
        }

        #region Not implemented or implemented partially
        void onConsentReady(AndroidJavaObject spConsents)
        {
            //TODO:
            //Util.Log("I've reached the C# onConsentReady");
            //try
            //{
            //unwrappedSpConsent is c# object which takes java.lang.Object in constructor; constructor accesses java.lang.Object fields and set result to C# object 
            //SpConsents unwrappedSpConsent = new SpConsents(spConsents);
            //Util.LogError("Works!");
            //ConsentMessenger.Broadcast<IOnConsentReadyEventHandler>(unwrappedSpConsent);
            //}
            //catch (Exception ex) { Util.LogError(ex.Message); }
        }

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