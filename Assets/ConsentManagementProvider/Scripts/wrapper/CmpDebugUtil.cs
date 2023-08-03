using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class CmpDebugUtil 
    {
        private static bool enableLogging = true;
        private static bool enableDebugging = true;

        static CmpDebugUtil()
        {
            EnableGarbageCollectorDebugging(enableDebugging);
            EnableCmpLogs(enableLogging);
        }

        public static void EnableCmpLogs(bool enable)
        {
            enableLogging = enable;
        }

        public static void EnableGarbageCollectorDebugging(bool enable)
        {
            enableDebugging = enable;
            AndroidJNIHelper.debug = enable;
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