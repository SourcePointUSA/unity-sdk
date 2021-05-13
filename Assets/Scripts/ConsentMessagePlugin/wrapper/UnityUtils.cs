using System;
using UnityEngine;

namespace GdprConsentLib
{
    public static class UnityUtils
    {
        const string UnityUtilsPackageName = "com.sourcepoint.cmplibrary.unity3d.UnityUtils";

        internal static AndroidJavaObject ConvertArrayToList(AndroidJavaObject[] array)
        {
            using (AndroidJavaClass UnityUtils = new AndroidJavaClass(UnityUtilsPackageName))
            {
                DebugUtil.Log("C# : passing Array to List conversion to Android's UnityUtils...");
                AndroidJavaObject list = UnityUtils.CallStatic<AndroidJavaObject>("arrayToList", new AndroidJavaObject[][] { array });
                return list;
            }
        }

        internal static Exception ConvertThrowableToError(AndroidJavaObject rawErr)
        {
            using (AndroidJavaClass UnityUtils = new AndroidJavaClass(UnityUtilsPackageName))
            {
                try
                {
                    DebugUtil.Log("C# : passing Throwable to Exception conversion to Android's UnityUtils...");
                    UnityUtils.CallStatic("throwableToException", rawErr);
                }
                catch (AndroidJavaException exception)
                {
                    return exception;
                }
                return new NotImplementedException();
            }
        }

        internal static void CallCustomConsentGDPR(AndroidJavaObject spConsentLib, string[] vendors, string[] categories, string[] legIntCategories, UnityCustomConsentGDPRProxy successCallback)
        {
            using (AndroidJavaClass UnityUtils = new AndroidJavaClass(UnityUtilsPackageName))
            {
                try
                {
                    DebugUtil.Log("C# : passing call of customConsentGDPR Android's UnityUtils...");
                    UnityUtils.CallStatic("callCustomConsentGDPR", spConsentLib, vendors, categories, legIntCategories, successCallback);
                }
                catch (AndroidJavaException exception)
                {
                    DebugUtil.LogError(exception.Message);
                }
            }
        }
    }
}