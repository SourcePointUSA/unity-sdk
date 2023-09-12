using ConsentManagementProviderLib.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentAction : IConsentEventHandler
    {
        void OnConsentAction(SpAction action);
    }
}