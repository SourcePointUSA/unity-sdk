using System;
using UnityEngine.EventSystems;

namespace ConsentManagementProviderLib
{
    public interface IOnConsentErrorEventHandler : IConsentEventHandler
    {
        void OnConsentError(Exception exception);
    }
}