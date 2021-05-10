using System;
using UnityEngine;

namespace GdprConsentLib
{
    public static class UnityUtils
    {
        internal static AndroidJavaObject ConvertArrayToList(AndroidJavaObject[] array)
        {
            AndroidJavaClass UnityUtils = new AndroidJavaClass("com.sourcepoint.cmplibrary.util.UnityUtils");
            Util.Log("C# : passing Array to List conversion to Android's UnityUtils...");
            AndroidJavaObject list = UnityUtils.CallStatic<AndroidJavaObject>("targetingParamArrayToList", new AndroidJavaObject[][] { array });
            return list;
        }

        internal static Exception ConvertThrowableToError(AndroidJavaObject rawErr)
        {
            AndroidJavaClass UnityUtils = new AndroidJavaClass("com.sourcepoint.cmplibrary.util.UnityUtils");
            try
            {
                Util.Log("C# : passing Throwable to Exception conversion to Android's UnityUtils...");
                UnityUtils.CallStatic("throwableToException", rawErr);
            }
            catch (AndroidJavaException exception)
            {
                return exception;
            }
            return new NotImplementedException();
        }
    }
}