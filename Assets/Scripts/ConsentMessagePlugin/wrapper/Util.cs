using System.Collections;
using UnityEngine;

namespace GdprConsentLib
{
    public static class Util 
    {
        internal static readonly bool enableLogging = false;

        public static void Log(string message)
        {
            if(enableLogging)
                Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (enableLogging)
                Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            //if(enableLogging)
                Debug.LogError(message);
        }
    }
}