using System;

namespace ConsentManagementProvider.Android
{
    internal static class AndroidConsentLifecycle
    {
        internal static void ScheduleClearAllData(
            Action<Action> runOnUiThread,
            Action<string> callConsentLibrary)
        {
            runOnUiThread(delegate { callConsentLibrary("clearLocalData"); });
        }
    }
}
