using ConsentManagementProvider.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProvider
{
    public interface IOnConsentUIReady : IConsentEventHandler
    {
        void OnConsentUIReady(/*AndroidJavaObject view*/);
    }
}