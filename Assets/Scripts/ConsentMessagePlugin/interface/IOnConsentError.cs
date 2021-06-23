using System;
using ConsentManagementProviderLib.EventHandlerInterface;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentError : IConsentEventHandler
    {
        void OnConsentError(Exception exception);
    }
}