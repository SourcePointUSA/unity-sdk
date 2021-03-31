using UnityEngine.EventSystems;

namespace GdprConsentLib
{
    public interface IOnConsentMessageReady : IConsentEventHandler
    {
        void OnConsentMessageReady(/*object spMessage*/);
    }
}