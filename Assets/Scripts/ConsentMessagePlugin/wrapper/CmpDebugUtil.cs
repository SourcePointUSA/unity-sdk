using UnityEngine;

namespace GdprConsentLib
{
    public static class CmpDebugUtil 
    {
        private static readonly bool enableLogging = false;
        private static readonly bool enableDebugging = false;

        static CmpDebugUtil()
        {
            EnableGarbageCollectorDebugging();
        }

        public static void EnableGarbageCollectorDebugging()
        {
            AndroidJNIHelper.debug = enableDebugging;
        }

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