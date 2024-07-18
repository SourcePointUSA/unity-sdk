﻿using System;
using System.Collections.Concurrent;
using ConsentManagementProvider.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProvider.Observer
{
    internal static class BroadcastEventDispatcher
    {
        public static ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        public static void Execute<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IConsentEventHandler
        {
            var handlers = BroadcastReceivers.GetHandlersForEvent<T>();
            if (handlers == null) return;
            CmpDebugUtil.Log($"{typeof(T).Name} has {handlers.Length} invokable instances");
            foreach (var handler in handlers)
            {
                actions.Enqueue(delegate { ExecuteEvents.Execute<T>(handler, eventData, functor); });
            }
        }

        public static void Execute(Action action)
        {
            actions.Enqueue(action);
        }
    }
}