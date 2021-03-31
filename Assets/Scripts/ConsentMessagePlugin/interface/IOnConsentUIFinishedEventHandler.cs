using UnityEngine.EventSystems;

namespace GdprConsentLib
{
    public interface IOnConsentUIFinishedEventHandler : IConsentEventHandler
    {
        void OnConsentUIFinished(/*AndroidJavaObject view*/);
    }    
}