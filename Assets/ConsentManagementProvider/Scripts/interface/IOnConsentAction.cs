using ConsentManagementProvider.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProvider
{
    public interface IOnConsentAction : IConsentEventHandler
    {
        void OnConsentAction(SpAction action);
    }
}