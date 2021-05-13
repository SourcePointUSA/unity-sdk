using System;
using System.Collections;
using UnityEngine;

namespace GdprConsentLib
{
    public class UnityCustomConsentGDPRProxy : AndroidJavaProxy
    {
        static readonly string NativeJavaInterfaceName = "com.sourcepoint.cmplibrary.unity3d.UnityCustomConsentGDPRProxy";
        Action<string> callback;

        public UnityCustomConsentGDPRProxy(Action<string> callback) : base(new AndroidJavaClass(NativeJavaInterfaceName)) 
        {
            this.callback = callback;
        }

        void transferCustomConsentToUnitySide(string spConsentsJson) //SPCustomConsents?
        {
            DebugUtil.Log("transferCustomConsentToUnitySide c#-side custom consent ->" + spConsentsJson.ToString());
            callback?.Invoke(spConsentsJson);
        }
    }
}