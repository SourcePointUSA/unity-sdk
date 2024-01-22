using System;
using System.Collections.Generic;
using System.Linq;
using ConsentManagementProviderLib.EventHandlerInterface;
using UnityEngine;

namespace ConsentManagementProviderLib.Observer
{
    internal static class BroadcastReceivers 
    {
        private static readonly IDictionary<Type, IList<GameObject>> BroadcastsReceivers = new Dictionary<Type, IList<GameObject>>();

        public static GameObject[] GetHandlersForEvent<T>() where T : IConsentEventHandler
        {
            lock (BroadcastsReceivers)
            {
                if (!BroadcastsReceivers.ContainsKey(typeof(T)))
                {
                    return null;
                }

                return BroadcastsReceivers[typeof(T)].ToArray();
            }
        }

        public static void RegisterBroadcastReceiver<T>(GameObject go) where T : IConsentEventHandler
        {
            lock (BroadcastsReceivers)
            {
                if (BroadcastsReceivers.ContainsKey(typeof(T)))
                {
                    BroadcastsReceivers[typeof(T)].Add(go);
                }
                else
                {
                    BroadcastsReceivers.Add(typeof(T), new List<GameObject>());
                    BroadcastsReceivers[typeof(T)].Add(go);
                }
            }
        }

        public static void UnregisterBroadcastReceiver<T>(GameObject go) where T : IConsentEventHandler
        {
            lock (BroadcastsReceivers)
            {
                if (BroadcastsReceivers.ContainsKey(typeof(T)) && BroadcastsReceivers[typeof(T)].Contains(go))
                {
                    BroadcastsReceivers[typeof(T)].Remove(go);
                }
                else
                {
                    CmpDebugUtil.LogWarning($"{go.name} is not subscribed to handle {typeof(T)}");
                }
            }
        }
    }
}