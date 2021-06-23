using ConsentManagementProviderLib.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentAction : IConsentEventHandler
    {
        void OnConsentAction(CONSENT_ACTION_TYPE action);
    }
}