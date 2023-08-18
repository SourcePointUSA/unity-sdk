using System;
using ConsentManagementProviderLib.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentSpFinished : IConsentEventHandler
    {
        void OnConsentSpFinished(SpConsents spConsent);
    }
}