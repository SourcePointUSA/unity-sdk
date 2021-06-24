using ConsentManagementProviderLib.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentUIFinished : IConsentEventHandler
    {
        void OnConsentUIFinished(/*AndroidJavaObject view*/);
    }    
}