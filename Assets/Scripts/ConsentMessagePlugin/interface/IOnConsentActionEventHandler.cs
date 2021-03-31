using UnityEngine.EventSystems;

namespace GdprConsentLib
{
    public interface IOnConsentActionEventHandler : IConsentEventHandler
    {
        void OnConsentAction(CONSENT_ACTION_TYPE action);
    }
}