using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentUIReadyEventHandler : IConsentEventHandler
    {
        void OnConsentUIReady(/*AndroidJavaObject view*/);
    }
}