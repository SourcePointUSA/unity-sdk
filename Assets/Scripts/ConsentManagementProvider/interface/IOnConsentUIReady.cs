using ConsentManagementProviderLib.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentUIReady : IConsentEventHandler
    {
        void OnConsentUIReady(/*AndroidJavaObject view*/);
    }
}