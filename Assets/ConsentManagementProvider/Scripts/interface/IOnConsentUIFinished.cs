using ConsentManagementProvider.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProvider
{
    public interface IOnConsentUIFinished : IConsentEventHandler
    {
        void OnConsentUIFinished(/*AndroidJavaObject view*/);
    }    
}