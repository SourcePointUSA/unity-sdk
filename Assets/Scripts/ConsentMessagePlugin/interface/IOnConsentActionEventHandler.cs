using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentActionEventHandler : IConsentEventHandler
    {
        void OnConsentAction(CONSENT_ACTION_TYPE action);
    }
}