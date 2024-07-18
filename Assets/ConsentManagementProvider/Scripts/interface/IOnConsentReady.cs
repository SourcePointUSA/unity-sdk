using ConsentManagementProvider.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProvider
{
    public interface IOnConsentReady : IConsentEventHandler
    {
        void OnConsentReady(SpConsents consents);
    }
}