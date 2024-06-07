using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class CmpDebugUtil 
    {
        private static bool enableLogging = false;
        private static bool enableDebugging = false;
        private static bool forceEnableSingleLog;

        public static bool IsLogging => enableLogging;
        
        private static bool IsLoggingEnabled
        {
            get
            {
                var result = enableLogging || forceEnableSingleLog;
                forceEnableSingleLog = false;
                return result;
            } 
        }
        
        static CmpDebugUtil()
        {
            EnableGarbageCollectorDebugging(enableDebugging);
            EnableCmpLogs(enableLogging);
        }

        public static void ForceEnableNextCmpLog() => forceEnableSingleLog = true;

        public static void EnableCmpLogs(bool enable) => enableLogging = enable;

        public static void EnableGarbageCollectorDebugging(bool enable)
        {
            enableDebugging = enable;
            AndroidJNIHelper.debug = enable;
        }

        public static void Log(string message)
        {
            if(IsLoggingEnabled)
                PrintLog(message);
        }

        public static void LogWarning(string message)
        {
            if (IsLoggingEnabled)
                Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            //if(EnableLogging)
                Debug.LogError(message);
        }
        
        /// <summary>
        /// Some logs we print are ENORMOUS and logcat shortens the lenght of a string.
        /// To workaround it, we'll use this method.
        /// </summary>
        private static void PrintLog(string message)
        {
            int maxLogSize = 1000;
            int start = 0;
            int end = 0;
            for(int i = 0; i <= (message.Length / maxLogSize)-1; i++) {
                start = i * maxLogSize;
                end = (i+1) * maxLogSize;
                end = end > message.Length ? message.Length : end;
                Debug.Log(message.Substring(start, maxLogSize));
            }
            Debug.Log(message.Substring(end));
        }
    }
}