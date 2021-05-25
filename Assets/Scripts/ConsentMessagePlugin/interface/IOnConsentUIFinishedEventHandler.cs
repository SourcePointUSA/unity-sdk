using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentUIFinishedEventHandler : IConsentEventHandler
    {
        void OnConsentUIFinished(/*AndroidJavaObject view*/);
    }    
}