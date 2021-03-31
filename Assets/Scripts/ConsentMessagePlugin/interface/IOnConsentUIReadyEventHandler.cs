using UnityEngine.EventSystems;

namespace GdprConsentLib
{
    public interface IOnConsentUIReadyEventHandler : IConsentEventHandler
    {
        void OnConsentUIReady(/*AndroidJavaObject view*/);
    }
}