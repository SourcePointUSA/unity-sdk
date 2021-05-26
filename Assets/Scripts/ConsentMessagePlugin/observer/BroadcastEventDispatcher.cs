using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public static class BroadcastEventDispatcher
    {
        public static Queue<Action> actions = new Queue<Action>();

        public static void Execute<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IConsentEventHandler
        {
            var handlers = BroadcastReceivers.GetHandlersForEvent<T>();
            if (handlers == null) return;
            CmpDebugUtil.Log($"{typeof(T).Name} has {handlers.Count} invokable instances");
            foreach (var handler in handlers)
            {
                actions.Enqueue(delegate { ExecuteEvents.Execute<T>(handler, eventData, functor); });
            }
        }
    }
}