using GdprConsentLib;
using UnityEngine.EventSystems;

public static class BroadcastExecuteEvents
{
    public static void Execute<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IConsentEventHandler
    {
        var handlers = BroadcastReceivers.GetHandlersForEvent<T>();
        if (handlers == null) return;
        CmpDebugUtil.Log($"{typeof(T).Name} has {handlers.Count} invokable instances");
        foreach (var handler in handlers)
        {
            ExecuteEvents.Execute<T>(handler, eventData, functor);
        }
    }
}