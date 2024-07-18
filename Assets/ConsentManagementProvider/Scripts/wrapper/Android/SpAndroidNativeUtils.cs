using System;
using UnityEngine;
using ConsentManagementProvider.Json;

namespace ConsentManagementProvider.Android
{
    internal class SpAndroidNativeUtils
    {
        private static AndroidJavaClass unityPlayerClass;
        private static AndroidJavaObject currentActivity;

        static SpAndroidNativeUtils()
        {
            unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        public static void ClearAllData()
        {
            AndroidJavaClass spUtilsClass = new AndroidJavaClass("com.sourcepoint.cmplibrary.util.SpUtils");

            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                spUtilsClass.CallStatic("clearAllData", currentActivity);
            }));
        }
    }
}