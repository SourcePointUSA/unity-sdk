using System;
using System.Collections;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public class CustomConsentClient : AndroidJavaProxy
    {
        static readonly string NativeJavaInterfaceName = "com.sourcepoint.cmplibrary.consent.CustomConsentClient";
        Action<string> callback;

        public CustomConsentClient(Action<string> callback) : base(new AndroidJavaClass(NativeJavaInterfaceName)) 
        {
            this.callback = callback;
        }

        void transferCustomConsentToUnity(string spConsentsJson) //TODO: SPCustomConsents
        {
            CmpDebugUtil.Log("transferCustomConsentToUnitySide c#-side custom consent ->" + spConsentsJson.ToString());
            callback?.Invoke(spConsentsJson);
        }
    }
}