using ConsentManagementProviderLib.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentReady : IConsentEventHandler
    {
        void OnConsentReady(SpConsents consents);
    }
}