using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentMessageReady : IConsentEventHandler
    {
        void OnConsentMessageReady(/*object spMessage*/);
    }
}