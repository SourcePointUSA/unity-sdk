using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentReadyEventHandler : IConsentEventHandler
    {
        void OnConsentReady(SpConsents consents);
    }
}