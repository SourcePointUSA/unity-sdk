using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

public class AndroidClearAllDataCompatibilityTests
{
    private const string AndroidConsentLifecycleTypeName =
        "ConsentManagementProvider.Android.AndroidConsentLifecycle";

    [Test]
    public void ClearAllDataQueuesLiveLibraryResetAheadOfSubsequentLoads()
    {
        var lifecycleType = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType(AndroidConsentLifecycleTypeName))
            .FirstOrDefault(type => type != null);
        Assert.That(lifecycleType, Is.Not.Null,
            "The Android clear-data lifecycle must remain available to the Android bridge.");

        var scheduleClearMethod = lifecycleType.GetMethod(
            "ScheduleClearAllData",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(scheduleClearMethod, Is.Not.Null,
            "ClearAllData must expose its UI-thread bridge operation for compatibility verification.");

        var uiThreadQueue = new Queue<Action>();
        var nativeCalls = new List<string>();
        Action<Action> runOnUiThread = action => uiThreadQueue.Enqueue(action);
        Action<string> callConsentLibrary = methodName => nativeCalls.Add(methodName);

        scheduleClearMethod.Invoke(null, new object[] { runOnUiThread, callConsentLibrary });

        Assert.That(nativeCalls, Is.Empty,
            "The native reset must not run before its Android UI-thread action is dispatched.");
        Assert.That(uiThreadQueue, Has.Count.EqualTo(1));

        uiThreadQueue.Enqueue(() => nativeCalls.Add("loadMessage"));
        while (uiThreadQueue.Count > 0)
        {
            uiThreadQueue.Dequeue()();
        }

        Assert.That(nativeCalls, Is.EqualTo(new[] { "clearLocalData", "loadMessage" }),
            "The live 7.12 consent library must reset before a subsequently queued load.");
    }
}
