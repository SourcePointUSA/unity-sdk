﻿using System;
using UnityEngine;
using System.Text.Json;

namespace ConsentManagementProviderLib
{
    public class CustomConsentClient : AndroidJavaProxy
    {
        static readonly string NativeJavaInterfaceName = "com.sourcepoint.cmplibrary.consent.CustomConsentClient";
        Action<SpGdprConsent> callback;

        public CustomConsentClient(Action<SpGdprConsent> callback) : base(new AndroidJavaClass(NativeJavaInterfaceName)) 
        {
            this.callback = callback;
        }

        void transferCustomConsentToUnity(string spConsentsJson)
        {
            CmpDebugUtil.Log("transferCustomConsentToUnitySide c#-side custom consent ->" + spConsentsJson.ToString());
            SpCustomConsentAndroid parsed = null;
            try
            {
                parsed = JsonSerializer.Deserialize<SpCustomConsentAndroid>(spConsentsJson);
            }
            catch (Exception ex)
            {
                Debug.LogError("Something went wrong while parsing the json data; null will be returned. \n Exception message: " + ex.Message);
            }
            finally
            {
                if (parsed == null)
                {
                    callback?.Invoke(null);
                }
                else
                {
                    callback?.Invoke(parsed.gdpr);
                }
            }
        }
    }
}