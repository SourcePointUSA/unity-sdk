﻿using System;
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
    }
}