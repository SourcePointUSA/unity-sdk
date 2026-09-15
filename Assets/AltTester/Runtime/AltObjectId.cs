using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltTester.AltTesterUnitySDK
{
    internal static class AltObjectId
    {
#if UNITY_6000_5_OR_NEWER
        private static readonly Dictionary<EntityId, int> protocolIds = new Dictionary<EntityId, int>();
        private static int nextProtocolId = 1;
#endif

        internal static int Get(UnityEngine.Object unityObject)
        {
#if UNITY_6000_5_OR_NEWER
            if (unityObject == null)
            {
                return 0;
            }

            var entityId = unityObject.GetEntityId();
            lock (protocolIds)
            {
                if (protocolIds.TryGetValue(entityId, out var protocolId))
                {
                    return protocolId;
                }

                if (nextProtocolId == int.MaxValue)
                {
                    throw new InvalidOperationException("AltTester protocol object IDs have been exhausted.");
                }

                protocolId = nextProtocolId++;
                protocolIds.Add(entityId, protocolId);
                return protocolId;
            }
#else
            return unityObject.GetInstanceID();
#endif
        }
    }
}
