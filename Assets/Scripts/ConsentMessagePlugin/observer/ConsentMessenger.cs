using System;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public static class ConsentMessenger
    {
        private static GameObject iOSListenerGO;
        
        static ConsentMessenger()
        {
#if UNITY_IOS && !UNITY_EDITOR_OSX
            CreateHelperIOSListener();
#endif
        }

        private static void CreateHelperIOSListener()
        {
            iOSListenerGO = new GameObject();
            iOSListenerGO.AddComponent<CMPiOSListenerHelper>();
        }

        public static void AddListener<T>(GameObject go) where T : IConsentEventHandler
        {
            BroadcastReceivers.RegisterBroadcastReceiver<T>(go);
        }

        public static void RemoveListener<T>(GameObject go) where T : IConsentEventHandler
        {
            BroadcastReceivers.UnregisterBroadcastReceiver<T>(go);
        }

        public static void Broadcast<T>(params object[] list) where T : IConsentEventHandler
        {
            CmpDebugUtil.LogWarning("T == " + typeof(T).Name);
            switch (typeof(T).Name)
            {
                //case IOnConsentMessageReady messReady: break; //TODO
                case nameof(IOnConsentReadyEventHandler):
                    //SpConsents consents = (SpConsents)list[0];
                    string jsonConsents = (string)list[0];
                    BroadcastEventDispatcher.Execute<IOnConsentReadyEventHandler>(null, (i, d) => i.OnConsentReady(jsonConsents));
                    break;
                case nameof(IOnConsentActionEventHandler):
                    CONSENT_ACTION_TYPE actionType = (CONSENT_ACTION_TYPE)list[0];
                    BroadcastEventDispatcher.Execute<IOnConsentActionEventHandler>(null, (i, d) => i.OnConsentAction(actionType));
                    break;
                case nameof(IOnConsentErrorEventHandler):
                    Exception exception= (Exception)list[0];
                    BroadcastEventDispatcher.Execute<IOnConsentErrorEventHandler>(null, (i, d) => i.OnConsentError(exception));
                    break;
                case nameof(IOnConsentUIReadyEventHandler):
                    BroadcastEventDispatcher.Execute<IOnConsentUIReadyEventHandler>(null, (i,d) => i.OnConsentUIReady());
                    break;
                case nameof(IOnConsentUIFinishedEventHandler):
                    BroadcastEventDispatcher.Execute<IOnConsentUIFinishedEventHandler>(null, (i,d) => i.OnConsentUIFinished());
                    break;
            }   
        }
    }
}